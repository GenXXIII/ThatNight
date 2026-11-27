"""Deterministic procedural horror & crying audio for Chapter 1 Forest Nightmare."""
import math
import random
import struct
import wave
from pathlib import Path

OUT = Path(__file__).resolve().parents[1] / "Assets/ChapterOne/Audio"
OUT.mkdir(parents=True, exist_ok=True)
RATE = 22050


def write_wav(name: str, samples: list[float]) -> None:
    path = OUT / name
    with wave.open(str(path), "wb") as stream:
        stream.setparams((1, 2, RATE, len(samples), "NONE", "not compressed"))
        stream.writeframes(
            b"".join(struct.pack("<h", int(max(-1.0, min(1.0, s)) * 32767)) for s in samples)
        )
    print(f"Generated {path} ({len(samples) / RATE:.2f}s)")


def biquad_bandpass(samples: list[float], freq: float, q: float) -> list[float]:
    w0 = 2.0 * math.pi * freq / RATE
    alpha = math.sin(w0) / (2.0 * q)
    b0 = alpha
    b2 = -alpha
    a0 = 1.0 + alpha
    a1 = -2.0 * math.cos(w0)
    a2 = 1.0 - alpha
    x1 = x2 = y1 = y2 = 0.0
    out = [0.0] * len(samples)
    for i, x0 in enumerate(samples):
        y0 = (b0 * x0 + b2 * x2 - a1 * y1 - a2 * y2) / a0
        out[i] = y0
        x2, x1 = x1, x0
        y2, y1 = y1, y0
    return out


def add_forest_reverb(samples: list[float], wet: float = 0.35) -> list[float]:
    taps = [(int(0.095 * RATE), 0.45), (int(0.19 * RATE), 0.30), (int(0.33 * RATE), 0.18), (int(0.51 * RATE), 0.10)]
    out = list(samples)
    for delay, gain in taps:
        for i in range(delay, len(samples)):
            out[i] += samples[i - delay] * gain * wet
    return out


def generate_woman_crying() -> None:
    """Haunting, trembling female sobbing/weeping loop (~12s) with formant vocal shaping."""
    rng = random.Random(20060414)
    duration = 12.0
    n = int(RATE * duration)
    glottal = [0.0] * n
    breath = [0.0] * n

    # 5 sobbing bursts across 12 seconds with trembling inhales between them
    sobs = [
        (0.40, 1.85, 470.0, 345.0, 0.90),
        (2.65, 4.10, 445.0, 320.0, 0.80),
        (4.80, 6.45, 505.0, 355.0, 0.98),
        (7.15, 8.70, 455.0, 330.0, 0.85),
        (9.35, 11.15, 485.0, 335.0, 0.92),
    ]
    inhales = [
        (1.95, 2.50, 0.55),
        (4.20, 4.68, 0.50),
        (6.55, 7.05, 0.62),
        (8.80, 9.25, 0.52),
        (11.25, 11.78, 0.48),
    ]

    phase = 0.0
    for i in range(n):
        t = i / RATE
        noise = rng.uniform(-1.0, 1.0)
        v_env = 0.0
        f0 = 420.0
        for s_start, s_end, f_hi, f_lo, strength in sobs:
            if s_start <= t <= s_end:
                u = (t - s_start) / (s_end - s_start)
                # Fast sad attack, weeping decay, trembling sob flutter (~7.5 Hz)
                shape = math.sin(min(1.0, u * 3.5) * math.pi * 0.5) * ((1.0 - u) ** 0.65)
                tremble = 0.68 + 0.32 * math.sin(2.0 * math.pi * 7.6 * (t - s_start))
                catch = 1.0 - 0.35 * max(0.0, math.sin(2.0 * math.pi * 3.8 * (t - s_start)))
                v_env = shape * tremble * catch * strength
                f0 = f_hi + (f_lo - f_hi) * (u ** 0.7) + math.sin(2.0 * math.pi * 5.8 * t) * 14.0
                break

        phase += 2.0 * math.pi * f0 / RATE
        # Rich asymmetric vocal fold wave (harmonics 1..6)
        wave_v = (
            math.sin(phase) * 0.52
            + math.sin(2.0 * phase + 0.2) * 0.30
            + math.sin(3.0 * phase + 0.5) * 0.18
            + math.sin(4.0 * phase) * 0.10
            + math.sin(5.0 * phase) * 0.05
        )
        glottal[i] = wave_v * v_env

        b_env = v_env * 0.42
        for in_start, in_end, strength in inhales:
            if in_start <= t <= in_end:
                u = (t - in_start) / (in_end - in_start)
                shudder = 0.65 + 0.35 * math.sin(2.0 * math.pi * 11.0 * u)
                b_env += math.sin(u * math.pi) * shudder * strength * 0.65
                break
        breath[i] = noise * b_env

    # Female sad vowel formants ("ahh / ohh / mmm") + breathy aspiration band
    f1 = biquad_bandpass(glottal, 660.0, 5.0)
    f2 = biquad_bandpass(glottal, 1280.0, 6.5)
    f3 = biquad_bandpass(glottal, 2550.0, 8.0)
    b_mid = biquad_bandpass(breath, 1050.0, 2.2)

    mixed = [0.0] * n
    for i in range(n):
        t = i / RATE
        edge = min(1.0, t / 0.35, (duration - t) / 0.35)
        s = (f1[i] * 1.15 + f2[i] * 0.72 + f3[i] * 0.28 + b_mid[i] * 0.85) * edge
        mixed[i] = math.tanh(s * 1.35) * 0.82

    reverb = add_forest_reverb(mixed, wet=0.42)
    peak = max(abs(x) for x in reverb) or 1.0
    write_wav("WomanCrying.wav", [x / peak * 0.88 for x in reverb])


def generate_neck_crack() -> None:
    """Eerie dry bone-snapping and twisting creaks for the ghost's 360 neck roll."""
    rng = random.Random(20061031)
    duration = 2.0
    n = int(RATE * duration)
    raw = [0.0] * n
    cracks = [0.18, 0.34, 0.56, 0.88, 1.14, 1.38, 1.62]
    for c_time in cracks:
        idx0 = int(c_time * RATE)
        for j in range(int(0.11 * RATE)):
            if idx0 + j >= n:
                break
            dt = j / RATE
            snap_env = math.exp(-dt * 42.0)
            pop = math.sin(2.0 * math.pi * (240.0 - dt * 900.0) * dt) * 0.6
            crunch = rng.uniform(-1.0, 1.0) * snap_env
            raw[idx0 + j] += (pop * math.exp(-dt * 28.0) + crunch) * 0.85

    # Low sinew/bone grinding undertone
    for i in range(n):
        t = i / RATE
        grind = math.sin(2.0 * math.pi * (92.0 + 35.0 * math.sin(7.0 * t)) * t)
        env = math.sin(min(1.0, t / 0.2) * math.pi * 0.5) * max(0.0, 1.0 - t / duration)
        raw[i] += grind * env * 0.18

    hi = biquad_bandpass(raw, 1650.0, 1.6)
    lo = biquad_bandpass(raw, 360.0, 2.0)
    mixed = [math.tanh((raw[i] * 0.35 + hi[i] * 0.85 + lo[i] * 0.65) * 1.4) for i in range(n)]
    peak = max(abs(x) for x in mixed) or 1.0
    write_wav("NeckCrack.wav", [x / peak * 0.90 for x in mixed])


def generate_shock_gasp() -> None:
    """Sharp startled wake-up gasp + pounding panicked heartbeat for waking from dream."""
    rng = random.Random(20060415)
    duration = 3.2
    n = int(RATE * duration)
    breath = [0.0] * n
    thumps = [0.0] * n

    for i in range(n):
        t = i / RATE
        # Sharp terrified inhale gasp (0.04s -> 0.55s) + shaky exhale (0.75s -> 1.5s)
        gasp_env = 0.0
        if 0.04 <= t <= 0.58:
            u = (t - 0.04) / 0.54
            gasp_env = math.sin(u * math.pi) ** 0.7 * 1.0
        elif 0.75 <= t <= 1.65:
            u = (t - 0.75) / 0.90
            gasp_env = math.sin(u * math.pi) * 0.42 * (0.75 + 0.25 * math.sin(2.0 * math.pi * 9.0 * u))
        vocal_catch = math.sin(2.0 * math.pi * 520.0 * t) * 0.14 if 0.08 <= t <= 0.36 else 0.0
        breath[i] = rng.uniform(-1.0, 1.0) * gasp_env + vocal_catch

        # Fast panicked heartbeat (lub-dub at ~135 BPM fading smoothly)
        beat = 0.0
        for b_t, amp in [(0.15, 0.95), (0.31, 0.65), (0.62, 0.85), (0.78, 0.55), (1.12, 0.70), (1.28, 0.45), (1.68, 0.52), (1.84, 0.32), (2.28, 0.35)]:
            if b_t <= t < b_t + 0.16:
                dt = t - b_t
                beat += math.sin(2.0 * math.pi * (62.0 - dt * 150.0) * dt) * math.exp(-dt * 26.0) * amp
        thumps[i] = beat

    gasp_bp = biquad_bandpass(breath, 1180.0, 1.8)
    out = [math.tanh((gasp_bp[i] * 0.95 + thumps[i] * 0.85) * 1.2) for i in range(n)]
    peak = max(abs(x) for x in out) or 1.0
    write_wav("ShockGasp.wav", [x / peak * 0.92 for x in out])


if __name__ == "__main__":
    generate_woman_crying()
    generate_neck_crack()
    generate_shock_gasp()
