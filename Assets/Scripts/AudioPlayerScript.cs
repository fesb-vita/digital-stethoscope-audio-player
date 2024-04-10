//This script allows you to toggle music to play and stop.
//Assign an AudioSource to a GameObject and attach an Audio Clip in the Audio Source. Attach this script to the GameObject.

using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.Networking;

public class AudioPlayerScript : MonoBehaviour
{

    public UnityEngine.UI.Text playText, currentTime, totalTime;
    public UnityEngine.UI.Slider timeline;
    public UnityEngine.UI.Image waveformImage;
    public UnityEngine.UI.Image waveformMask;
    public string argUrl;
    
    Texture2D waveform;
    // class containing information about a clip from the audiofile
    //  - starting at start_percentage and ending at end_percentage
    //  - with class label class_index


    [Serializable]
    public class ClipClass
    {
        public float start_percentage, end_percentage;
        public int class_index;
    }
    // class containing a list of classified clips from the audio file
    //  - audio file with the name audioName
    //  - list of ClipClass objects clips
    //  - list of class colors, indexed by class index, i.e. list[class_index]
    [Serializable]
    public class AudioClassification
    {
        public string audioName;
        public List<ClipClass> clips;
        public List<Color> class_colors;
    }
    [Serializable]
    public class AudioAnnotation
    {
        public float from;
        public float to;
        public int type;
    }

    [SerializeField] AudioClip clip;
    [SerializeField] AudioSource source;


    private string url;
    private string metaUrl;

    private int zoom = 100, ZOOM_STEP = 10, MIN_ZOOM = 100, MAX_ZOOM = 1000;
    private int offset = 0, OFFSET_STEP = 10, MIN_OFFSET = 0, MAX_OFFSET = 100;
    private int graph_width, graph_height;
    private AudioClassification ac;

    public void StartSetClip(string argUrl)
    {
        StartSetClip(argUrl, "");
    }
    public void StartSetClip(string argUrl, string argUrlMeta)
    {

        ac = new AudioClassification
        {
            audioName = "blablabla", // We dont care about this
            clips = new List<ClipClass>()
        };

        ac.class_colors = new List<Color>
        {
            Color.green,
            Color.blue,
            Color.yellow,
            Color.red,
        };
       

        graph_width = (int)waveformImage.rectTransform.rect.width;
        graph_height = (int)waveformImage.rectTransform.rect.height;
        url = argUrl;
        metaUrl = argUrlMeta;


        StartCoroutine(SetSourceAudioClip());
    }

    float percent_start;
    float percent_end;
    bool waveformChangedBool = false;
    private void UpdateWaveform()
    {
        timeline.value = source.time;

        int minutes = Mathf.FloorToInt(source.time / 60F);
        int seconds = Mathf.FloorToInt(source.time - minutes * 60);

        currentTime.text = string.Format("{0:0}:{1:00}", minutes, seconds);


        percent_start = offset * 0.01f;
        percent_end = percent_start + 100.0f / zoom;
        if (percent_end - percent_start < 0.001f) percent_end = percent_start + 0.001f;
        if (percent_start < 0.0f) percent_start = 0.0f;
        if (percent_end > 1.0f) percent_end = 1.0f;

        graph_width = (int)waveformImage.rectTransform.rect.width;
        graph_height = (int)waveformImage.rectTransform.rect.height;

        if (waveformChangedBool)
        {
            waveform = PaintWaveformSpectrum(source.clip, graph_width, graph_height, Color.red, percent_start, percent_end, ac);
            waveformImage.sprite = Sprite.Create(waveform, new Rect(0, 0, waveform.width, waveform.height), Vector2.one * 0.5f);
            waveformChangedBool = false;
        }

        UpdateWaveformMask();
    }
    private void UpdateWaveformMask()
    {
        if (source.time / source.clip.length <= percent_start)
        {
            waveformMask.rectTransform.sizeDelta = new Vector2((int)waveformImage.preferredWidth, waveformImage.preferredHeight);
        }
        else if (source.time / source.clip.length <= percent_end)
        {
            waveformMask.rectTransform.sizeDelta = new Vector2((int)waveformImage.preferredWidth * ((percent_end - source.time / source.clip.length) / (percent_end - percent_start)), waveformImage.preferredHeight);
        }
        else
        {
            waveformMask.rectTransform.sizeDelta = new Vector2(0, waveformImage.preferredHeight);
        }
    }

    private void Start()
    {
        if (argUrl == null || argUrl == "")
        {
            argUrl = "https://cdn.pixabay.com/download/audio/2023/11/15/audio_a50bbe2bd1.mp3?filename=biodynamic-impact-braam-tonal-dark-176441.mp3";
        }
        StartSetClip(argUrl);
    }
    private void Update()
    {
        if (source.clip != null && source.isPlaying)
        {
            UpdateWaveform();
        }
    }

    public void PlayMusic()
    {
        playText.text = "Pause";
        if (source.isPlaying) return;

        StartCoroutine(WaitForMusicEnd());
    }

    public void StopMusic()
    {
        playText.text = "Play";
        source.Pause();
    }


    public void ToggleStopPause()
    {
        if (source.isPlaying) StopMusic();
        else PlayMusic();
    }

    public void Rewind()
    {
        source.time = 0;
    }

    public void Forward10Secs()
    {
        if (source.clip.length > source.time + 10)
        {
            source.time += 10;
        }
        else
        {
            StopMusic();
            source.time = source.clip.length;
        }
    }

    public void Backward10Secs()
    {
        if (source.time - 10 >= 0)
        {
            source.time -= 10;
        }
        else
        {
            source.time = 0;
        }
    }

    public void ZoomIn()
    {
        if (zoom < MAX_ZOOM) zoom += ZOOM_STEP; else zoom = MAX_ZOOM;
        waveformChangedBool = true;
        UpdateWaveform();
    }

    public void ZoomOut()
    {
        if (zoom > MIN_ZOOM) zoom -= ZOOM_STEP; else zoom = MIN_ZOOM;
        waveformChangedBool = true;
        UpdateWaveform();
    }

    public void OffsetInc()
    {
        if ((offset + OFFSET_STEP) / 100.0f + 100.0f / zoom < 1.0f) offset += OFFSET_STEP;
        waveformChangedBool = true;
        UpdateWaveform();
    }

    public void OffsetDec()
    {
        if ((offset - OFFSET_STEP) / 100.0f > 0.0f) offset -= OFFSET_STEP; else offset = 0;
        waveformChangedBool = true;
        UpdateWaveform();
    }

    public void SetTime()
    {
        source.time = timeline.value;
        if (!source.isPlaying)
        {
            UpdateWaveformMask();
        }
    }

    IEnumerator WaitForMusicEnd()
    {
        while (source.isPlaying)
        {
            yield return null;
        }
        source.Play();
    }

    /*public void SetSourceFromByteFile(string songName)
    {
        byte[] songData = File.ReadAllBytes("path to file goes here");
        using (Stream s = new MemoryStream(songData))
        {
            AudioClip audioClip = AudioClip.Create(songName, songData.Length / 4, 1, 48000, false);
            float[] f = ConvertByteToFloat(songData);
            audioClip.SetData(f, 0);
            source.clip = audioClip;
        }
    }*/

    IEnumerator SetSourceAudioClip()
    {
        string[] splitURL = url.Split('.');
        string filetype = splitURL[^1];//simplified splitURL[splitURL.Length-1]
        AudioType type = AudioType.UNKNOWN;
        switch (filetype)
        {
            case "mp3":
                type = AudioType.MPEG;
                Debug.Log("mp3");
                break;
            case "wav":
                type = AudioType.WAV;
                Debug.Log("wav");
                break;
            default:
                Debug.LogError("Unsupported audio file type!");
                break;
        }

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, type))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
                source.clip = myClip;
            }
        }
        // @Luka Added
        if (!metaUrl.Equals(""))
        {
            using (UnityWebRequest www = UnityWebRequest.Get(metaUrl))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    var text = www.downloadHandler.text;
                    List<AudioAnnotation> listOfAudioAnnotations = JsonConvert.DeserializeObject<List<AudioAnnotation>>(text.ToString());
                    foreach (AudioAnnotation annotation in listOfAudioAnnotations)
                    {

                        ClipClass clip = new()
                        {
                            class_index = annotation.type,
                            start_percentage = annotation.from / source.clip.length,
                            end_percentage = annotation.to / source.clip.length
                        };
                        ac.clips.Add(clip);
                    }
                }
            }
        }
        else
        {
            ac = JsonUtility.FromJson<AudioClassification>(File.ReadAllText("Assets/Data/classified_clips.json"));
        }


        timeline.minValue = 0;
        timeline.maxValue = source.clip.length;

        int minutes = Mathf.FloorToInt(source.clip.length / 60F);
        int seconds = Mathf.FloorToInt(source.clip.length - minutes * 60);

        totalTime.text = string.Format("{0:0}:{1:00}", minutes, seconds);

        PlayMusic();
        waveformChangedBool = true;
    }

    private float[] ConvertByteToFloat(byte[] array)
    {
        var floatArray = new float[array.Length / 4];
        Buffer.BlockCopy(array, 0, floatArray, 0, array.Length);
        return floatArray;
    }


    public Texture2D PaintWaveformSpectrum(AudioClip audio, int width, int height, Color col, float percent_start, float percent_end)
    {
        Texture2D tex = new(width, height, TextureFormat.RGBA32, false);
        float[] temp_samples = new float[audio.samples];
        float[] waveform = new float[width];
        audio.GetData(temp_samples, 0);

        int start_index = Mathf.FloorToInt(percent_start * temp_samples.Length);
        int end_index = Mathf.FloorToInt(percent_end * temp_samples.Length);
        float[] samples = new float[end_index - start_index];

        for (int x = start_index; x < end_index; x++)
            samples[x - start_index] = temp_samples[x];

        int packSize = (samples.Length / width) + 1;
        int s = 0;
        for (int i = 0; i < samples.Length; i += packSize)
        {
            waveform[s] = Mathf.Abs(samples[i]);
            s++;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tex.SetPixel(x, y, Color.clear);
            }
        }

        for (int x = 0; x < waveform.Length; x++)
        {
            for (int y = 0; y <= waveform[x] * ((float)height * .75f); y++)
            {
                tex.SetPixel(x, (height / 2) + y, col);
                tex.SetPixel(x, (height / 2) - y, col);
            }
        }
        tex.Apply();

        return tex;
    }

    public Texture2D PaintWaveformSpectrum(AudioClip audio, int width, int height, Color col, float percent_start, float percent_end, AudioClassification ac)
    {
        Texture2D tex = new(width, height, TextureFormat.RGBA32, false);
        float[] temp_samples = new float[audio.samples];
        float[] waveform = new float[width];
        audio.GetData(temp_samples, 0);

        int start_index = Mathf.FloorToInt(percent_start * temp_samples.Length);
        int end_index = Mathf.FloorToInt(percent_end * temp_samples.Length);
        float[] samples = new float[end_index - start_index];

        for (int x = start_index; x < end_index; x++)
            samples[x - start_index] = temp_samples[x];

        int packSize = (samples.Length / width) + 1;
        int s = 0;
        for (int i = 0; i < samples.Length; i += packSize)
        {
            waveform[s] = Mathf.Abs(samples[i]);
            s++;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tex.SetPixel(x, y, Color.clear);
            }
        }
        tex.Apply();
        tex = PaintClassifiedWaveformSpectrum(tex, ac, percent_start, percent_end, 0.4f);

        for (int x = 0; x < waveform.Length; x++)
        {
            for (int y = 0; y <= waveform[x] * ((float)height * .75f); y++)
            {
                tex.SetPixel(x, (height / 2) + y, col);
                tex.SetPixel(x, (height / 2) - y, col);
            }
        }
        tex.Apply();

        return tex;
    }

    public Texture2D PaintClassifiedWaveformSpectrum(Texture2D original, AudioClassification ac, float percent_start, float percent_end, float alpha)
    {
        // TODO add zoom/offset action
        int height = original.height;
        int start, end;

        // color the audio texture in strips based on the class indices
        foreach (ClipClass i in ac.clips)
        {
            // entire clip outside current zoom and offset
            if (i.start_percentage > percent_end) continue;
            if (i.end_percentage < percent_start) continue;

            // part of clip outside current zoom and offset
            if (i.start_percentage < percent_start) start = 0;
            else start = Mathf.FloorToInt((i.start_percentage - percent_start) / (percent_end - percent_start) * original.width);
            if (i.end_percentage > percent_end) end = original.width;
            else end = Mathf.FloorToInt((i.end_percentage - percent_start) / (percent_end - percent_start) * original.width);

            // sanity check
            if (start < 0) start = 0;
            if (end > original.width) end = original.width;

            Color color = ac.class_colors[i.class_index];
            color.a = alpha;
            //Debug.Log(i.class_index+" "+start+" "+end+" "+color);
            for (int x = start; x < end; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    original.SetPixel(x, y, color + original.GetPixel(x, y));
                }
            }
        }
        original.Apply();
        return original;
    }
}