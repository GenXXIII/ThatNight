"""Deterministic, original ambient beds; no third-party recording or license dependency."""
import math
import random
import struct
import wave
from pathlib import Path

out = Path(__file__).resolve().parents[1] / 'Assets/ChapterOne/Forest/Audio'
out.mkdir(parents=True, exist_ok=True)
rate, seconds = 22050, 24
count = rate * seconds
rng = random.Random(20061121)

def write(name, samples):
    # Fade the seam; low output level is deliberate for a quiet forest.
    with wave.open(str(out / name), 'wb') as stream:
        stream.setparams((1, 2, rate, count, 'NONE', 'not compressed'))
        stream.writeframes(b''.join(struct.pack('<h', int(max(-1, min(1, value)) * 32767)) for value in samples))

wind, insects = [], []
low, lower = 0.0, 0.0
for i in range(count):
    t = i / rate
    edge = min(1, t / .8, (seconds - t) / .8)
    noise = rng.uniform(-1, 1)
    low += .025 * (noise - low)
    lower += .0015 * (noise - lower)
    gust = .55 + .23 * math.sin(2 * math.pi * t / 12) + .12 * math.sin(2 * math.pi * t / 8)
    wind.append((low * 1.8 + lower * 3) * gust * edge)
    chirp = 0
    for frequency, period, offset in [(3280, 2.4, .1), (4350, 3, .7), (2870, 4, 1.4)]:
        phase = (t + offset) % period
        gate = max(0, math.sin(phase * math.pi / .48)) ** 2 if phase < .48 else 0
        pulse = .5 + .5 * math.sin(2 * math.pi * 36 * t)
        chirp += math.sin(2 * math.pi * frequency * t) * gate * pulse * .12
    insects.append((chirp + noise * .004) * edge)
write('ForestWind.wav', wind)
write('NightInsects.wav', insects)
print('Wrote two original 24-second mono ambient beds.')
