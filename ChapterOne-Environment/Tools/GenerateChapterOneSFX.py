"""
Procedural audio generator for THAT NIGHT — Chapter 1: The Wedding
Generates authentic rural Cambodian atmosphere sound effects:
- DistantRooster.wav (authentic morning village rooster crow at 4:00 AM)
- ClockTick.wav (vintage wooden clock ticking)
- DeepRumble.wav (sub-bass acoustic resonance from beneath stilt house)
- Scream.wav (supernatural horror shockwave crescendo)
- MorningBirds.wav (peaceful morning Cambodian countryside birds)
- TraditionalWedding.wav (Phleng Kar Khmer wedding music motif)
"""
import math
import random
import struct
import wave
from pathlib import Path

out = Path(__file__).resolve().parents[1] / 'Assets/ChapterOne/Audio'
out.mkdir(parents=True, exist_ok=True)
rate = 22050

def write_wav(name, samples):
    filepath = out / name
    with wave.open(str(filepath), 'wb') as stream:
        stream.setparams((1, 2, rate, len(samples), 'NONE', 'not compressed'))
        stream.writeframes(b''.join(struct.pack('<h', int(max(-1, min(1, value)) * 32767)) for value in samples))
    print(f"Generated {filepath} ({len(samples)/rate:.1f}s)")

# 1. Distant Rooster (Crow at 4:00 AM)
# Typical rooster crow has 4 syllables: "er-er-ER-ER-oo"
rooster_seconds = 4.0
rooster_samples = []
rooster_len = int(rate * rooster_seconds)
for i in range(rooster_len):
    t = i / rate
    # Pitch curve: starts ~450Hz, climbs to ~850Hz, slides down to ~520Hz
    if 0.5 <= t < 0.9: # Syllable 1
        freq = 480 + 60 * (t - 0.5) / 0.4
        amp = math.sin((t - 0.5) / 0.4 * math.pi) * 0.45
    elif 1.0 <= t < 1.35: # Syllable 2
        freq = 550 + 100 * (t - 1.0) / 0.35
        amp = math.sin((t - 1.0) / 0.35 * math.pi) * 0.55
    elif 1.45 <= t < 2.1: # Syllable 3 (peak)
        freq = 780 + 120 * math.sin((t - 1.45) / 0.65 * math.pi)
        amp = math.sin((t - 1.45) / 0.65 * math.pi) * 0.75
    elif 2.15 <= t < 3.2: # Syllable 4 (long tail)
        p = (t - 2.15) / 1.05
        freq = 680 - 240 * p
        amp = math.sin(p * math.pi) * 0.6 * (1.0 - p * 0.5)
    else:
        freq = 0
        amp = 0
    
    val = 0.0
    if amp > 0:
        # Harmonics for authentic vocal timbre
        val = (math.sin(2 * math.pi * freq * t) * 0.6 +
               math.sin(4 * math.pi * freq * t) * 0.3 +
               math.sin(6 * math.pi * freq * t) * 0.1) * amp
        # Distance lowpass & reverb simulation
        val += (random.uniform(-0.015, 0.015))
    rooster_samples.append(val * 0.6)
write_wav('DistantRooster.wav', rooster_samples)

# 2. Clock Tick (Vintage wooden bedroom clock)
tick_seconds = 1.0
tick_samples = []
tick_len = int(rate * tick_seconds)
for i in range(tick_len):
    t = i / rate
    val = 0.0
    # Tick at 0.1s
    if 0.1 <= t < 0.14:
        dt = t - 0.1
        env = math.exp(-dt * 90)
        val = math.sin(2 * math.pi * 1250 * dt) * env * 0.8
        val += math.sin(2 * math.pi * 340 * dt) * env * 0.4
    # Tock at 0.6s
    elif 0.6 <= t < 0.64:
        dt = t - 0.6
        env = math.exp(-dt * 95)
        val = math.sin(2 * math.pi * 980 * dt) * env * 0.7
        val += math.sin(2 * math.pi * 280 * dt) * env * 0.5
    tick_samples.append(val)
write_wav('ClockTick.wav', tick_samples)

# 3. Deep Acoustic Rumble (Resonance beneath stilt house floorboards)
rumble_seconds = 5.0
rumble_samples = []
rumble_len = int(rate * rumble_seconds)
for i in range(rumble_len):
    t = i / rate
    env = math.sin(t / rumble_seconds * math.pi) ** 1.5
    # Sub-bass frequencies 32Hz, 48Hz, 64Hz
    sub = (math.sin(2 * math.pi * 34 * t) * 0.55 +
           math.sin(2 * math.pi * 48 * t + 0.4) * 0.35 +
           math.sin(2 * math.pi * 68 * t + 0.8) * 0.20)
    noise = random.uniform(-0.08, 0.08)
    rumble_samples.append((sub + noise) * env * 0.85)
write_wav('DeepRumble.wav', rumble_samples)

# 4. Horror Scream & Supernatural Distortion
scream_seconds = 3.0
scream_samples = []
scream_len = int(rate * scream_seconds)
for i in range(scream_len):
    t = i / rate
    # Sharp attack at 0.2s, swelling crescendo
    if t < 0.15:
        env = t / 0.15
    else:
        env = math.exp(-(t - 0.15) * 1.8)
    
    # Sharp dissonant acoustic scream & metal screech
    pitch = 850 + math.sin(t * 22) * 120 + t * 300
    screech = (math.sin(2 * math.pi * pitch * t) * 0.4 +
               math.sin(2 * math.pi * (pitch * 1.414) * t) * 0.35 + # tritone dissonance
               math.sin(2 * math.pi * (pitch * 2.82) * t) * 0.25)
    noise = random.uniform(-0.25, 0.25)
    scream_samples.append((screech + noise) * env * 0.95)
write_wav('Scream.wav', scream_samples)

# 5. Morning Countryside Birds (Battambang rural dawn)
birds_seconds = 16.0
birds_samples = []
birds_len = int(rate * birds_seconds)
rng = random.Random(42)
for i in range(birds_len):
    t = i / rate
    val = 0.0
    # Gentle wind background
    val += math.sin(2 * math.pi * 0.15 * t) * 0.02 + random.uniform(-0.01, 0.01)
    
    # Periodic bird chirps at specific intervals
    for b_time, b_freq in [(1.2, 2800), (3.4, 3200), (5.8, 2600), (8.2, 3400), (10.5, 2900), (13.1, 3100)]:
        if b_time <= t < b_time + 0.35:
            dt = t - b_time
            chirp_p = dt / 0.35
            f = b_freq + math.sin(chirp_p * math.pi * 4) * 400
            env = math.sin(chirp_p * math.pi)
            val += math.sin(2 * math.pi * f * dt) * env * 0.18
            
    birds_samples.append(val)
write_wav('MorningBirds.wav', birds_samples)

# 6. Traditional Khmer Wedding Motif (Phleng Kar Pinpeat melody)
# Uses authentic pentatonic Khmer scale: D, E, F#, A, B
wedding_seconds = 18.0
wedding_samples = []
wedding_len = int(rate * wedding_seconds)
# Pentatonic frequencies in Hz
notes = [293.66, 329.63, 369.99, 440.00, 493.88, 587.33] # D4, E4, F#4, A4, B4, D5
melody_pattern = [
    (0.0, 0, 0.8), (0.9, 1, 0.8), (1.8, 2, 1.2), (3.2, 3, 1.0),
    (4.4, 4, 1.4), (6.0, 3, 0.8), (7.0, 2, 0.8), (8.0, 1, 1.5),
    (9.8, 2, 0.8), (10.8, 3, 0.8), (11.8, 4, 1.2), (13.2, 5, 1.5),
    (15.0, 3, 1.0), (16.2, 0, 1.8)
]
for i in range(wedding_len):
    t = i / rate
    val = 0.0
    for start, note_idx, dur in melody_pattern:
        if start <= t < start + dur:
            dt = t - start
            f = notes[note_idx]
            # Tro / Roneat wooden bell harmonic envelope
            env = math.sin(dt / dur * math.pi) * math.exp(-dt * 1.5)
            # Bamboo mallet harmonic
            note_val = (math.sin(2 * math.pi * f * dt) * 0.6 +
                        math.sin(4 * math.pi * f * dt) * 0.25 +
                        math.sin(6 * math.pi * f * dt) * 0.15)
            val += note_val * env * 0.28
    # Subtle acoustic background murmur
    val += random.uniform(-0.015, 0.015)
    wedding_samples.append(val)
write_wav('KhmerWeddingMusic.wav', wedding_samples)

print("All Chapter 1 audio assets generated successfully!")
