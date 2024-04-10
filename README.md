<h1>DigitalStethoscopeAudioPlayer</h1>
Unity version: 2022:3:11f1<br>
Ovaj Unity projekt omogućuje reproducijranje i snimanje zvukova.<br>
Unutar ovog projekta se nalaze dvije scene unutar <strong>Assets/Scenes</strong>:<br>
1. <strong>ClipPlayerScene</strong> (reprodukcija zvukova)<br>
2. <strong>AudioRecordPlayerScene</strong> (snimanje zvukova)<br>
<br>
<h2>Obe scene</h2>
<picture><img alt="Oba prefaba" src="https://media.githubusercontent.com/media/fesb-vita/digital-stethoscope-audio-player/main/ScreenshotsForGit/ClipPlayerScreenShot.JPG"></picture><br>
Obe scene imaju iste GameObject-e s jedinom razlikom u glavnom objektu (u <strong>ClipPlayerScene</strong> ClipPlayerPrefab ima <em>AudioPlayerScript</em>, u <strong>AudioRecordPlayerScene</strong> AudioRecorderPrefab ima <em>AudioPlayerStreamScript</em>)<br>
Sve navedeni buttoni pozivaju istu funkciju obe scene:<br>
<ol>
  <li>
Button Play/Pause poziva funkciju <em>ToggleStopPause</em>
<picture><img alt="Play/PauseButton" src="https://media.githubusercontent.com/media/fesb-vita/digital-stethoscope-audio-player/main/ScreenshotsForGit/Play_StopButtonComponent.JPG"></picture><br>
  </li>
  <li>
Button -10sec poziva funkciju <em>Backward10Secs</em>
  </li>
  <li>
Button +10sec poziva funkciju <em>Foreward10Secs</em>
  </li>
  <li>
Button Rewind poziva funkciju <em>Rewind</em>
  </li>
  <li>
Button OffsetBack poziva funkciju <em>OffsetDec</em>
  </li>
  <li>
Button OffsetForward poziva funkciju <em>OffsetInc</em>
  </li>
  <li>
Button ZoomIn poziva funkciju <em>ZoomIn</em>
  </li>
  <li>
Button ZoomOut poziva funkciju <em>ZoomOut</em>
  </li>
  <li>
Slider Timeline kad se pomakne/prijeni poziva funkciju <em>SetTime</em>
  </li>
</ol>
<h2>ClipPlayerScene</h2>
<picture><img alt="ClipPlayerScene" src="https://media.githubusercontent.com/media/fesb-vita/digital-stethoscope-audio-player/main/ScreenshotsForGit/ClipPlayerScene.JPG"></picture><br>

