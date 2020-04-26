//2dAudio

using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]

public class Audio2d : MonoBehaviour
{
    private static Audio2d instance = null;
    [SerializeField] private AudioSource audioSource = null;
    private readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>(); //代入不可 連想配列

    public static Audio2d Instance //インスタンスを返す
    {
        get 
        {
            return instance;
        }
    }

    private void Awake()
    {
        if(null != instance)
        {
            Destroy(gameObject); //既にインスタンスがある場合は自分を破棄する
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this; //インスタンス代入
        AudioClip[] audioclips = Resources.LoadAll<AudioClip>(""); //Resourcesディレクトリ以下のAudioClipをすべて取得
        foreach(AudioClip clip in audioclips)
        {
            clips.Add(clip.name, clip);
        }
    }

    public void Play(string clipname) //指定した名前の音楽ファイル再生
    {
        if(clips.ContainsKey(clipname) == false) //存在しない名前を指定した場合エラー
        {
            throw new Exception("Sound" + clipname + "is not defined");
        }
        audioSource.clip = clips[clipname]; //差し替える
        audioSource.Play();
    }
}
