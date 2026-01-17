using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioScript : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioClip NextPlay_clip;
    static bool Audio_Loaded = false;
    static float Current_Volume;
    bool Function_BGM_VolumeDown = false;
    const float Down_Value = 0.25f;
    [SerializeField] private AudioMixer audioMixer; // 作成したMixerをアタッチ
    [SerializeField, Range(0f, 1f)] private float MaxVolumeLimit = 0.5f;

    // Start is called before the first frame update
    void Awake()
    {
        //重複防止処理
        if (Audio_Loaded == true)
        {
            Destroy(this.gameObject);
            return;
        }
        Audio_Loaded = true;
        DontDestroyOnLoad(this);
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
    }

    void Update()
    {
        if (Function_BGM_VolumeDown == true)
        {
            if (Return_AudioPlaying() == true)
            {
                Fadeout_AudioVolume();
            }
            else
            {
                Function_BGM_VolumeDown = false;
                Change_PlayAudio(NextPlay_clip);
            }
        }
    }

    //音楽再生
    void PlayAudio(AudioClip audio)
    {
        Function_BGM_VolumeDown = false;
        audioSource.Stop();
        audioSource.clip = audio;
        audioSource.loop = true;
        audioSource.volume = Current_Volume;
        audioMixer.SetFloat("VolumeMaster", Get_AudioRatio(Current_Volume));

        audioSource.Play();
    }

    //音楽をすぐに変更する
    public void Change_PlayAudio(AudioClip audio)
    {
        if (audioSource.clip == audio && audio == null) return;
        PlayAudio(audio);
        Debug.Log("Audio Changed");
    }

    //音楽をフェードアウトしてから変更する
    public void Change_PlayAudio_with_VolumeDown(AudioClip audio)
    {
        if (audioSource.clip == audio || Function_BGM_VolumeDown == true) return;
        NextPlay_clip = audio;
        Function_BGM_VolumeDown = true;
    }

    //音楽の再生を停止
    public void Stop_Audio()
    {
        Function_BGM_VolumeDown = false;
        audioSource.Stop();
        Reset_AudioValue();
    }

    //パラメータを初期化
    public void Reset_AudioValue()
    {
        audioSource.volume = Current_Volume;
    }
    public void Reset_AudioValue(float Vol)
    {
        Current_Volume = Vol;
        audioSource.volume = Vol;
    }

    //任意のパラメータを設定（ただし、エフェクト実行中は無視）
    public void Set_MasterParameter(float Volume)
    {
        if (Function_BGM_VolumeDown == true) return;
        Current_Volume = Volume;
        audioSource.volume = Volume;

        float volumeInDecibels = Get_AudioRatio(Volume);

        // 第1引数：Exposed Parametersで設定した名前
        // 第2引数：設定したいデシベル値
        audioMixer.SetFloat("VolumeMaster", volumeInDecibels);
    }

    private float Get_AudioRatio(float value)
    {
        float vol = value * MaxVolumeLimit;
        // Mathf.Log10(0) はエラーになるため、最小値に注意
        if (value <= 0.0001f)
        {
            // スライダーが0の時は無音(-80dB)にする
            return -80f;
        }
        else
        {
            // 0.0〜1.0 をデシベル変換する公式: 20 * log10(値)
            return Mathf.Log10(value) * 20f;
        }
    }

    //フェードアウト処理
    public void Set_FadeoutVolume_Function()
    {
        Function_BGM_VolumeDown = true;
    }
    void Fadeout_AudioVolume()
    {
        if (audioSource.volume <= 0) return;

        audioSource.volume -= Down_Value * Time.deltaTime;
        if (audioSource.volume <= 0) { Stop_Audio(); }

    }

    public void Fadeout_AudioPitch()
    {
        if (audioSource.pitch > 0)
        {
            audioSource.pitch -= Down_Value * Time.deltaTime;
            if (audioSource.pitch <= 0) { Stop_Audio(); }
        }
    }

    //再生中かを判定
    public bool Return_AudioPlaying() { return audioSource.isPlaying; }

    //現在の音量
    public float Get_AudioVolume()
    {
        return audioSource.volume;
    }
}
