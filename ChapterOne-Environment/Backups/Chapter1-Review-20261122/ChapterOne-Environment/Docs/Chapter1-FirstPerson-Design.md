# THAT NIGHT — Chapter 1: The Wedding
## Master First-Person Dialogue, Story Flow & Production Specification

> **CORE CANON & PERSPECTIVE DIRECTIVES**:
> 1. **Perspective**: 100% **First-Person Gameplay** through the eyes of the **Bride**.
> 2. **Character Canon**:
>    * **The Bride**: Main playable protagonist.
>    * **The Mother**: The bride's **biological mother** (STRICT: The mother is NOT the aunt).
>    * **The Cousin (Opharn)**: The bride's male cousin away on an important investigation project.
>    * **The Groom**: The bride's future husband.
> 3. **Design Philosophy**: The player must fall in love with the wedding, family, and cultural warmth before the horror strikes. The disappearance must be shocking, tragic, and completely unexplained in Chapter 1.

---

## 1. Scene Flow & Verbatim Dialogue

```mermaid
flowchart TD
    S1["Scene 01: 4:00 AM Opening Cinematic<br/><i>Director camera into Bride's eyes</i>"] --> S2["Scene 02: Morning Gameplay<br/><i>Waking up & Mother calling</i>"]
    S2 --> S3["Scene 03: Living Area<br/><i>Mother and Daughter conversation</i>"]
    S3 --> S4["Scene 04: The Phone Call<br/><i>Emotional call with Cousin Opharn</i>"]
    S4 --> S5["Scene 05: Wedding Preparation<br/><i>Dressing table & Mirror with Mother</i>"]
    S5 --> S6["Scene 06: Traditional Ceremony<br/><i>Procession, Groom dialogue & Blessings</i>"]
    S6 --> S7["Scene 07: Wedding Reception<br/><i>Romvong dance, Banquet & Goodnight to Mother</i>"]
    S7 --> S8["Scene 08: The Disappearance<br/><i>Bedroom disturbance, Blackout & Empty room</i>"]
    S8 --> S9["Scene 09: Chapter Ending<br/><i>Crane shot over dark jungle & Title drop</i>"]
```

---

### Scene 01 — Opening Cinematic: 4:00 AM
* **Time**: 4:00 AM.
* **Atmosphere Stage**: `AtmosphereStage.Dawn4AM` (Pre-dawn blue, morning mist, distant village birds & cicadas).
* **Shot List**:
  * **Shot 01 (Exterior Wide)**: Stilt house exterior before sunrise against dark blue sky.
  * **Shot 02 (Slow Dolly)**: Camera approaches the house across the quiet grounds.
  * **Shot 03 (Interior Tracking)**: Glides through the veranda and quiet central living area.
  * **Shot 04 (Bedroom Medium)**: Enters the bridal bedroom, revealing the bride sleeping under the canopy net.
  * **Shot 05 (Close-up)**: Bride slowly opens her eyes.
  * **Shot 06 (Detail Shot)**: Vintage wooden clock on wall ticking at **04:00 AM**.
  * **Shot 07 (First-Person Transition)**: Camera smoothly aligns with bride's eyes. Gameplay begins.

---

### Scene 02 — Morning Gameplay: Waking Up
* **Perspective**: First-Person (Bride's POV).
* **Actions**: Stand up from bed, look around bedroom, approach bedroom door, push door open.
* **Dialogue**:
  * **Mother (outside bedroom):** *"You're awake already?"*
  * **Bride:** *"Yes, Mom. I'm coming."*
  * **Mother:** *"Today is your big day. Don't stay in bed too long."*
  * **Bride:** *"I know, Mom."*
  * **Mother:** *"Come downstairs when you're ready. We still have so much to do."*
  * **Bride:** *"Okay!"*

---

### Scene 03 — Living Area: Mother & Daughter
* **Location**: Central living hall (*Rong Doung*).
* **Actions**: Walk into living room, approach Mother who is organizing ceremonial offerings and wedding arrangements.
* **Dialogue**:
  * **Mother:** *"There you are."*
  * **Bride:** *"What are you doing, Mom?"*
  * **Mother:** *"Checking everything for today. I don't want anything to be forgotten."*
  * **Bride:** *"You've been awake since early morning?"*
  * **Mother:** *"Of course. Do you think your wedding prepares itself?"*
  * **Bride:** *"Haha. You should have woken me up."*
  * **Mother:** *"You needed your sleep. Today will be a long day."*
  * **Bride:** *"Is everything ready?"*
  * **Mother:** *"Almost everything. The guests will start arriving later."*
  * **Bride:** *"I'm a little nervous."*
  * **Mother:** *"That's normal, sweetheart."*
  * **Bride:** *"What if something goes wrong?"*
  * **Mother:** *"Then we'll handle it together. Don't worry."*
  * **Bride:** *"Okay, Mom."*
  * **Mother:** *"Oh, and check your phone. You might have someone calling you."*

---

### Scene 04 — The Phone Call: Male Cousin (Opharn)
* **Location**: Living room central tea table (beside porcelain tea set & floral thermos) by the window.
* **Hand-Phone Specification**:
  * **Model**: Authentic 2006 classic button **Nokia** candybar phone (graphite/navy housing, rubber keypad, lanyard eyelet).
  * **Screen**: Backlit monochrome green LCD displaying signal bars, battery icon, and **`INCOMING CALL / Opharn (Cousin)`**.
  * **Audio**: Iconic 2006 monophonic/polyphonic Nokia ringtone (`NokiaRingtone2006.wav`) and vibrating rumble against the polished timber table (`NokiaVibration.wav`).
* **First-Person Handheld Experience**:
  1. The phone rings and vibrates on the tea table after Mother mentions someone might be calling.
  2. Prompt: `[E] Answer Nokia Phone`.
  3. The bride's hand reaches down, picks up the Nokia phone, and raises it into first-person view in front of her ear/sightline.
  4. The green LCD screen lights up with caller ID and call timer (`00:01`, `00:02`...).
  5. Looking out through the open wooden shutters into the cool dawn mist, she listens to Opharn's voice over the earpiece.
  6. Player retains full mouse-look control while holding the phone.
  7. When the conversation finishes, the classic Nokia two-beep call-ended tone plays (`NokiaCallEnded.wav`), screen reads `CALL ENDED`, and the bride gently sets the phone down.
* **Dialogue**:
  * **Bride:** *"Hello?"*
  * **Cousin:** *"Hey. Are you busy?"*
  * **Bride:** *"Not really. I've been waiting for your call."*
  * **Cousin:** *"Happy wedding day."*
  * **Bride:** *"That's all? You aren't even coming?"*
  * **Cousin:** *"I'm sorry."*
  * **Bride:** *"I really thought you would be here."*
  * **Cousin:** *"I wanted to. More than you know."*
  * **Bride:** *"I've been looking forward to today. I thought we'd finally get to spend some time together again."*
  * **Cousin:** *"I know. I wanted to see you in your wedding dress. I wanted to be there with everyone."*
  * **Bride:** *"Then why are you still there?"*
  * **Cousin:** *"The investigation isn't finished. We've found something, and I can't leave it right now."*
  * **Bride:** *"You always have something important to do."*
  * **Cousin:** *"I know I keep disappointing you. I'm sorry."*
  * **Bride:** *"I just wanted you here. That's all."*
  * **Cousin:** *"I promise I'll come see you as soon as I finish this."*
  * **Bride:** *"You better. I'm going to hold you to that promise."*
  * **Cousin:** *"I will. Now go enjoy your wedding. Don't let me ruin your day."*
  * **Bride:** *"I wish you were here."*
  * **Cousin:** *"I wish I was there too."*
  * *[Short silence]*
  * **Bride:** *"Take care of yourself, okay?"*
  * **Cousin:** *"You too. And congratulations."*
  * *[Call ends. Bride lowers phone slowly. Gentle breath.]*
  * **Mother (calling from another room):** *"Sweetheart! Are you ready?"*
  * **Bride:** *"Almost, Mom!"*

---

### Scene 05 — Wedding Preparation: Mother & Daughter
* **Location**: Bridal Bedroom dressing table and mirror.
* **Actions**: Return to bedroom, sit before the dressing mirror. Mother joins her, adjusting the gold jewelry and silk *Sbai*. Mirror reflects the bride's transformation into a Khmer bride.
* **Dialogue**:
  * **Mother:** *"Come here, sweetheart. Let's get you ready."*
  * **Bride:** *"Mom, is my dress ready?"*
  * **Mother:** *"Everything is ready. I checked it twice."*
  * **Bride:** *"You're more nervous than I am."*
  * **Mother:** *"Of course I am. My daughter is getting married today."*
  * **Bride:** *"You're making it sound like I'm leaving forever."*
  * **Mother:** *"You're starting a new life. That's different."*
  * **Bride:** *"I'll still come home and visit you."*
  * **Mother:** *"You better. I don't want you forgetting your mother."*
  * **Bride:** *"Never."*
  * **Mother:** *"Look at you. My little girl is growing up."*
  * **Bride:** *"Mom, don't cry."*
  * **Mother:** *"I'm not crying."*
  * **Bride:** *"You are!"*
  * **Mother:** *"I'm just happy, sweetheart."*
  * **Bride:** *"I love you, Mom."*
  * **Mother:** *"I love you too."*

---

### Scene 06 — Traditional Wedding Ceremony
* **Atmosphere Stage**: `AtmosphereStage.AfternoonCelebration` (Bright golden daylight, festive wedding marquee).
* **Audio**: Authentic Khmer wedding orchestra (*Phleng Kar* — Pinpeat, Tro, Roneat Ek).
* **Rites**: Groom procession, fruit offerings, hair cutting (*Gaat Sah*), wrist-tying (*Sompeas Ptem*) with sacred red thread and *Pka Sla* flowers.
* **Dialogue (Mother & Bride)**:
  * **Mother:** *"Are you ready?"*
  * **Bride:** *"I think so."*
  * **Mother:** *"Don't be nervous. Just smile."*
  * **Bride:** *"You're staying with me, right?"*
  * **Mother:** *"Of course. I'm right here."*
  * **Bride:** *"Thank you, Mom."*
  * **Mother:** *"Now go. Everyone is waiting."*
* **Dialogue (Bride & Groom)**:
  * **Groom:** *"You look beautiful."*
  * **Bride:** *"Thank you."*
  * **Groom:** *"Are you nervous?"*
  * **Bride:** *"A little. What about you?"*
  * **Groom:** *"Honestly? Very."*
  * **Bride:** *"I thought you would be confident."*
  * **Groom:** *"Not when I'm standing in front of you."*
  * **Bride:** *"Haha. You're silly."*
  * **Groom:** *"But I'm happy."*
  * **Bride:** *"Me too."*
* **Guest Ambience**: Spatial audio greetings ("Congratulations!", "They look wonderful together!", "May they have a happy life together!").

---

### Scene 07 — Wedding Reception: Evening
* **Atmosphere Stage**: `AtmosphereStage.EveningWarm` (Warm tungsten string bulbs, dusk to evening).
* **Actions**: Walk around tables, observe dancing (*Romvong*), greet relatives.
* **Dialogue (Mother & Daughter)**:
  * **Mother:** *"Are you enjoying yourself?"*
  * **Bride:** *"Yes, Mom. It's been wonderful."*
  * **Mother:** *"I'm glad."*
  * **Bride:** *"I wish everyone could be here."*
  * **Mother:** *"I know who you're thinking about."*
  * **Bride:** *"He really wanted to come."*
  * **Mother:** *"I'm sure he did. Don't be sad today."*
  * **Bride:** *"I know."*
  * **Mother:** *"Today is your day. Enjoy every moment."*
* **Dialogue (Bride & Groom)**:
  * **Groom:** *"You've been smiling all evening."*
  * **Bride:** *"It's my wedding. I'm allowed to."*
  * **Groom:** *"You look tired, though."*
  * **Bride:** *"A little."*
  * **Groom:** *"It's been a long day. Shall we go inside?"*
  * **Bride:** *"Okay. Just let me say goodbye to Mom."*
* **Dialogue (Goodnight to Mother)**:
  * **Bride:** *"Mom, we're going inside now."*
  * **Mother:** *"Alright, sweetheart. Get some rest."*
  * **Bride:** *"You should rest too."*
  * **Mother:** *"I will. Don't worry about me."*
  * **Bride:** *"Goodnight, Mom."*
  * **Mother:** *"Goodnight, my daughter."*

---

### Scene 08 — The Disappearance: Horror Climax
* **Atmosphere Stage**: `AtmosphereStage.NightHorror` (Late night, darkness outside).
* **Progression**:
  * **Phase 1 (Entering Room)**: Door closes, wedding music becomes muffled through timber walls.
    * **Groom:** *"Finally, some quiet."*
    * **Bride:** *"It's been a long day."*
    * **Groom:** *"A good one, though."*
    * **Bride:** *"The best."*
  * **Phase 2 (Disturbance)**: Deep acoustic resonance from beneath the stilt house floorboards. The room becomes unnaturally quiet. Light bulb flickers.
    * **Groom:** *"Did you hear that?"*
    * **Bride:** *"Hear what?"*
    * **Groom:** *"I thought someone was outside."*
    * **Bride:** *"Maybe it's just the guests."*
    * *[Sudden loud anomalous noise from below]*
    * **Groom:** *"Wait..."*
  * **Phase 3 (Blackout & Scream)**: Violent unnatural shockwave shakes room, camera distorts violently, sudden blackout, crash sound, sharp bloodcurdling scream!
  * **Phase 4 (Guest Reaction Outside)**:
    * **Guest:** *"What was that?!"*
    * **Mother:** *"That came from their room!"*
    * **Guest:** *"Open the door!"*
    * **Mother:** *"Where is my daughter?!"*
  * **Phase 5 (Empty Room)**:
    * Door bursts open with flashlight beams.
    * **The room is completely empty.**
    * Bride and groom are gone without a trace; wedding jewelry and belongings remain untouched.
    * **Mother (crying desperately):** *"Sweetheart?! ... Please answer me! ... No... no, this can't be happening!"*
    * Cut to black.

---

### Scene 09 — Final Cinematic & Title Reveal
* **Shot List**:
  * **Shot 01 (Static Wide)**: Relatives frantically searching around the empty room and hallway.
  * **Shot 02 (Slow Backward Dolly)**: Camera leaves the bedroom door.
  * **Shot 03 (Tracking)**: Floats through the dark stilt house hallway.
  * **Shot 04 (Exterior)**: Camera passes out through the open veranda into the cold night air.
  * **Shot 05 (Crane Upward)**: Ascends above the timber roof tiles.
  * **Shot 06 (Aerial Wide)**: Lonely illuminated house surrounded by vast, pitch-black Cambodian jungle.
  * **Shot 07 (Fade to Black)**: Screen fades to pure black.
  * **Shot 08 (Title Reveal)**:
    ```
    A CAMBODIAN SUPERNATURAL HORROR GAME

    THAT NIGHT

    CHAPTER ONE — THE WEDDING
    ```

---

## 2. Technical Presentation Rules (Unity 6)
1. **Dialogue UI & Presentation**:
   * Voice acting + clean, legible subtitles (Cambodian & English localization ready).
   * Spatial 3D audio for dialogue and village ambience.
   * NPCs turn their head/upper body to face the player's camera naturally when addressing the bride.
2. **First-Person Immersion**:
   * Player can freely look around during dialogue; movement can be gently clamped or guided during focused emotional beats.
   * No third-person perspective cuts during playable moments.
   * Natural eye level at **1.54m** for authentic female stature.
