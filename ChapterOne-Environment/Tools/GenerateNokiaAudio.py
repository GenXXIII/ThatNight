"""Synthesizes authentic 2006 Nokia monophonic ringtone, call-end tone, and table vibration."""
import math
import struct
import wave
from pathlib import Path

out_dir = Path(__file__).resolve().parents[1] / 'Assets/ChapterOne/Audio'
out_dir.mkdir(parents=True, exist_ok=True)
rate = 22050

def write_wav(filename, samples):
    filepath = out_dir / filename
    with wave.open(str(filepath), 'wb') as stream:
        stream.setparams((1, 2, rate, len(samples), 'NONE', 'not compressed'))
        stream.writeframes(b''.join(struct.pack('<h', int(max(-1, min(1, s)) * 32767)) for s in samples))
    print(f"Wrote {filename} ({len(samples)/rate:.2f}s)")

# 1. Iconic 2006 Nokia Ringtone (Monophonic / 8-bit square/triangle blend)
# Notes: E5, D5, F#4, G#4, C#5, B4, D4, E4, B4, A4, C#4, E4, A4
# Frequencies (Hz):
notes = [
    (1318.51, 0.14), # E6
    (1174.66, 0.14), # D6
    (739.99,  0.28), # F#5
    (830.61,  0.28), # G#5
    (1108.73, 0.14), # C#6
    (987.77,  0.14), # B5
    (587.33,  0.28), # D5
    (659.25,  0.28), # E5
    (987.77,  0.14), # B5
    (880.00,  0.14), # A5
    (554.37,  0.28), # C#5
    (659.25,  0.28), # E5
    (880.00,  0.55), # A5
]

ringtone_samples = []
for freq, dur in notes:
    note_samples = int(rate * dur)
    for i in range(note_samples):
        t = i / rate
        # Classic 2006 Nokia buzzer envelope: sharp attack, slight decay
        env = max(0, min(1, (dur - t) / 0.02)) if (dur - t) < 0.02 else min(1, t / 0.005)
        # 8-bit hybrid square wave with soft harmonics
        p = (freq * t) % 1.0
        wave_val = 0.6 if p < 0.5 else -0.6
        wave_val += 0.25 * math.sin(2 * math.pi * freq * t)
        ringtone_samples.append(wave_val * env * 0.45)
    # brief gap between notes (articulation)
    ringtone_samples.extend([0.0] * int(rate * 0.02))

# Repeat once for full ring cycle + pause
one_cycle = list(ringtone_samples)
ringtone_samples.extend([0.0] * int(rate * 0.8)) # 0.8s pause between rings
ringtone_samples.extend(one_cycle)
write_wav('NokiaRingtone2006.wav', ringtone_samples)

# 2. Table Vibration (Buzzing on polished wood)
vib_samples = []
vib_duration = 3.5
total_vib = int(rate * vib_duration)
for i in range(total_vib):
    t = i / rate
    cycle_pos = t % 1.2 # 0.7s buzz, 0.5s pause
    if cycle_pos < 0.65:
        # 140 Hz vibration motor rumble with table rattle harmonics
        motor = math.sin(2 * math.pi * 140 * t) * 0.4
        wood_rattle = math.sin(2 * math.pi * 280 * t) * 0.2 + (math.sin(2 * math.pi * 560 * t) * 0.1)
        vib_samples.append((motor + wood_rattle) * 0.35)
    else:
        vib_samples.append(0.0)
write_wav('NokiaVibration.wav', vib_samples)

# 3. Nokia Call Ended Tone (Two short beeps)
beep_samples = []
for _ in range(2):
    for i in range(int(rate * 0.08)):
        t = i / rate
        beep_samples.append(math.sin(2 * math.pi * 920 * t) * 0.35)
    beep_samples.extend([0.0] * int(rate * 0.06))
write_wav('NokiaCallEnded.wav', beep_samples)
