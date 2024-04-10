//This script allows you to toggle music to play and stop.
//Assign an AudioSource to a GameObject and attach an Audio Clip in the Audio Source. Attach this script to the GameObject.

using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.UIElements;

using static System.Net.Mime.MediaTypeNames;

public class AudioPlayerStreamScript : MonoBehaviour
{

    [SerializeField] AudioClip clip;
    [SerializeField] AudioSource source;

    public UnityEngine.UI.Text playText, currentTime, totalTime;
    public UnityEngine.UI.Slider timeline;
    public UnityEngine.UI.Image waveformImage;
    public UnityEngine.UI.Image waveformMask;
    Texture2D waveform;


    private int zoom = 100, ZOOM_STEP = 10, MIN_ZOOM = 100, MAX_ZOOM = 1000;
    private int offset = 0, OFFSET_STEP = 10, MIN_OFFSET = 0, MAX_OFFSET = 100;
    private int graph_width, graph_height;

    void Start()
    {
        graph_width = (int)waveformImage.rectTransform.rect.width;
        graph_height = (int)waveformImage.rectTransform.rect.height;


        SetSourceAudioClip();
    }

    private void LateUpdate()
    {
        if (source.clip && source.isPlaying)
        {
            //Debug.Log(source.time);
            //timeline.value = source.time;

            int minutes = Mathf.FloorToInt(source.time / 60F);
            int seconds = Mathf.FloorToInt(source.time - minutes * 60);

            currentTime.text = string.Format("{0:0}:{1:00}", minutes, seconds);


            float percent_start = offset * 0.01f;
            float percent_end = percent_start + 100.0f / zoom;
            if (percent_end - percent_start < 0.001f) percent_end = percent_start + 0.001f;
            if (percent_start < 0.0f) percent_start = 0.0f;
            if (percent_end > 1.0f) percent_end = 1.0f;


            //waveform = PaintWaveformSpectrum(source.clip, graph_width, graph_height, Color.red, percent_start, percent_end);
            int maxs = 15;
            waveform = PaintWaveformSpectrum(source, graph_width, graph_height, Color.red, maxs);
            waveformImage.sprite = Sprite.Create(waveform, new Rect(0, 0, waveform.width , waveform.height), Vector2.one * 0.5f);

            // added
            //waveformImage.rectTransform.localScale = new Vector3(10.0f/zoom, 1, 1);
            //waveformMask.rectTransform.localScale = new Vector3(10.0f / zoom, 1, 1);
            //waveformImage.rectTransform.localPosition = new Vector3(offset, 0, 0);
            //waveformMask.rectTransform.localPosition = new Vector3(offset, 0, 0);


            if (Microphone.GetPosition(null) < maxs * 44100)
                waveformMask.rectTransform.sizeDelta = new Vector2((int)waveformImage.rectTransform.sizeDelta.x * ((1 - Microphone.GetPosition(null)/ (maxs*44100) )), waveformMask.rectTransform.sizeDelta.y);
            else
                waveformMask.rectTransform.sizeDelta = new Vector2((int)waveformImage.rectTransform.sizeDelta.x * ((1)), waveformMask.rectTransform.sizeDelta.y);
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
        if (Microphone.IsRecording(null))
        {
            Microphone.End(null);
            playText.text = "Start";
            source.Pause();
            source.Stop();
            string filepath = "";
            byte[] bytes = WavUtility.FromAudioClip(source.clip, out filepath, true);
            
            //UploadStetoscopeRecording(filepath); TO IMPLEMENT
        }
        else
        {
            SetSourceAudioClip();
        }
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
    }

    public void ZoomOut()
    {
        if (zoom > MIN_ZOOM) zoom -= ZOOM_STEP; else zoom = MIN_ZOOM;
    }

    public void OffsetInc()
    {
        if ((offset + OFFSET_STEP) / 100.0f + 100.0f / zoom < 1.0f) offset += OFFSET_STEP;
    }

    public void OffsetDec()
    {
        if ((offset - OFFSET_STEP) / 100.0f > 0.0f) offset -= OFFSET_STEP; else offset = 0;
    }

    public void SetTime()
    {
        source.time = timeline.value;
    }

    IEnumerator WaitForMusicEnd()
    {
        while (source.isPlaying)
        {
            yield return null;
        }
        source.Play();
    }

    void SetSourceAudioClip()
    {
        // stream audio source setup
        if (Microphone.devices.Length <= 0)
        {
            Debug.LogWarning("No microphone connected!");
        } else
        {
            // maximum stream length is 1h

            source.clip = Microphone.Start(null, true, 60, 44100);
            source.Play();
        }


        timeline.minValue = 0;
        timeline.maxValue = source.clip.length;

        int minutes = Mathf.FloorToInt(source.clip.length / 60F);
        int seconds = Mathf.FloorToInt(source.clip.length - minutes * 60);

        totalTime.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        //PlayMusic();
    }

    private float[] ConvertByteToFloat(byte[] array)
    {
        var floatArray = new float[array.Length / 4];
        Buffer.BlockCopy(array, 0, floatArray, 0, array.Length);
        return floatArray;
    }


    public Texture2D PaintWaveformSpectrum(AudioSource source, int width, int height, Color col, int maxs)
    {

        AudioClip audio = source.clip;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

        
        float[] samples;

        var fs = 44100;

        var position = Microphone.GetPosition(null);


        if (position > maxs * fs)
        {
            samples = new float[position + fs/2];
            //Debug.Log($"#2 {samples.Length}");
        } else
        {
            samples = new float[maxs * fs];
            //Debug.Log($"#1 {samples.Length}");
        }


        float[] waveform = new float[width];
        audio.GetData(samples, 0);

        //int start_index = 0 * temp_samples.Length;
        //int end_index = 1 * temp_samples.Length;
        //float[] samples = new float[end_index - start_index];

        //for (int x = start_index; x < end_index; x++)
        //    samples[x - start_index] = temp_samples[x];

        int packSize = (samples.Length / width);
        if (packSize  < 1) { packSize = 1; }
        int s = 0;
        for (int i = 0; i < samples.Length; i += packSize)
        {
            if (s >= 700) break;
            
            var a =  Mathf.Abs(samples[i]);

            waveform[s] = a;
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
}