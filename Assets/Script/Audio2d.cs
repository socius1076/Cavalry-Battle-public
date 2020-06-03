using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]

public class Audio2d : MonoBehaviour
{
    private static Audio2d instance = null;
    [SerializeField] private AudioSource audioSource = null;
    private readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();

    //インスタンスを返す
    public static Audio2d Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if(instance != null)
        {
            //既にインスタンスがある場合は自分を破棄する
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        //インスタンス代入
        instance = this;
        //Resourcesディレクトリ以下のAudioClipをすべて取得
        AudioClip[] audioclips = Resources.LoadAll<AudioClip>("");
        foreach(AudioClip clip in audioclips)
        {
            clips.Add(clip.name, clip);
        }
    }

    //指定した名前の音楽ファイル再生
    public void Play(string clipname)
    {
        //存在しない名前を指定した場合エラー
        if(!clips.ContainsKey(clipname))
        {
            throw new Exception("Sound" + clipname + "is not defined");
        }
        audioSource.clip = clips[clipname];
        audioSource.Play();
    }
}
