<h1>DigitalStethoscopeAudioPlayer</h1>
Unity version: 2022:3:11f1<br>
Ovaj Unity projekt omogućuje reproducijranje i snimanje zvukova.<br>
Unutar ovog projekta se nalaze dvije scene unutar <strong>Assets/Scenes</strong>:<br>
1. <strong>ClipPlayerScene</strong> (reprodukcija zvukova)<br>
2. <strong>AudioRecordPlayerScene</strong> (snimanje zvukova)<br>
<h2>Obe scene</h2>
<picture><img alt="Oba prefaba" src="https://media.githubusercontent.com/media/fesb-vita/digital-stethoscope-audio-player/main/ScreenshotsForGit/ClipPlayerScreenShot.JPG"></picture><br>
Obe scene imaju iste GameObject-e s jedinom razlikom u glavnom objektu (u <strong>ClipPlayerScene</strong> ClipPlayerPrefab ima <em>AudioPlayerScript</em>, u <strong>AudioRecordPlayerScene</strong> AudioRecorderPrefab ima <em>AudioPlayerStreamScript</em>).<br>
Sve navedeni buttoni pozivaju istu funkciju obe scene:<br>
<ol>
  <li>
Button Play/Pause poziva funkciju <em>ToggleStopPause</em><br>
<picture><img alt="Play/PauseButton" src="https://media.githubusercontent.com/media/fesb-vita/digital-stethoscope-audio-player/main/ScreenshotsForGit/Play_StopButtonComponent.JPG"></picture><br>
  </li>
  <li>
Button -10sec poziva funkciju <em>Backward10Secs</em>.
  </li>
  <li>
Button +10sec poziva funkciju <em>Foreward10Secs</em>.
  </li>
  <li>
Button Rewind poziva funkciju <em>Rewind</em>.
  </li>
  <li>
Button OffsetBack poziva funkciju <em>OffsetDec</em>.
  </li>
  <li>
Button OffsetForward poziva funkciju <em>OffsetInc</em>.
  </li>
  <li>
Button ZoomIn poziva funkciju <em>ZoomIn</em>.
  </li>
  <li>
Button ZoomOut poziva funkciju <em>ZoomOut</em>.
  </li>
  <li>
Slider Timeline kad se pomakne/promijeni poziva funkciju <em>SetTime</em>.
  </li>
  <li>
<em>Waveform</em> i <em>WaveformMask</em> objekti su zaduženi za prikazivanje koji dio zvuka koji se reprodura i za bojanje za klasificirane dijelove zvuka.<br>
      <img alt="WaveformComponents" src="https://media.githubusercontent.com/media/fesb-vita/digital-stethoscope-audio-player/main/ScreenshotsForGit/WaveformComponents.JPG"/><img alt="WaveformMaskComponents" src="https://media.githubusercontent.com/media/fesb-vita/digital-stethoscope-audio-player/main/ScreenshotsForGit/WaveformMaskComponents.JPG"/>
  </li>
</ol>
<h2>ClipPlayerScene</h2>
<picture><img alt="ClipPlayerScene" src="https://media.githubusercontent.com/media/fesb-vita/digital-stethoscope-audio-player/main/ScreenshotsForGit/ClipPlayerScene.JPG"></picture><br>
Unutar ove scene je implementirana reprodukcija zvukova u ovom slučaju preko URL-a.<br>
<picture><img alt="ClipPlayerPrefabComponent" src="https://media.githubusercontent.com/media/fesb-vita/digital-stethoscope-audio-player/main/ScreenshotsForGit/ClipPlayerPrefabComponents.JPG"></picture><br>
Link koji se ubaci u polje ArgUrl i važeći link koji pušta nekavi zvuk će reproducirat taj zvuk kad se pritisne Unity-ev PlayTest. Ujedno će se obojati u ovisnosti kako je definiran file <em>Assets/Data/classified_clips.txt datoteka</em><br>
<picture><img alt="ClipPlayerPlayingClip" src="https://media.githubusercontent.com/media/fesb-vita/digital-stethoscope-audio-player/main/ScreenshotsForGit/ClipPlayerPlayingClip.JPG"></picture><br>
<h2>AudioRecordPlayerClipScene</h2>
<picture><img alt="AudioRecordPlayerScene" src="https://media.githubusercontent.com/media/fesb-vita/digital-stethoscope-audio-player/main/ScreenshotsForGit/AudioRecordPlayerScene.JPG"></picture><br>
Unutar ove scene je implementirano snimanje zvukova preko mikrofona/stetoskopa koji se spojen na računalo koji može snimat zvuk.<br>
<picture><img alt="AudioRedorderPrefabComponents" src="https://media.githubusercontent.com/media/fesb-vita/digital-stethoscope-audio-player/main/ScreenshotsForGit/AudioRecorderPrefab.JPG"></picture><br>
<picture><img alt="AudioRecorderRecordingVoice" src="https://media.githubusercontent.com/media/fesb-vita/digital-stethoscope-audio-player/main/ScreenshotsForGit/AudioRecorderRecordingVoice.JPG"></picture><br>
<br>
<br>
