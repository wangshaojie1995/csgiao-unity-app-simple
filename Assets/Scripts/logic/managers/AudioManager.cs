using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assist
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); //加载新的scene时，不要销毁的object （unity的公共静态空间）
            }
            else
            {
                Destroy(gameObject);
                
            }
        }
        
        public Sound[] musicSounds, sfxSounds;
        public AudioSource musicSource, sfxSource;
        
        public void PlayMusic(string name)
        {
            Sound s = Array.Find(musicSounds, x => x.name == name);
            if (s == null)
            {
                Debug.Log("Sound Not Found");
            }
            else
            {
                musicSource.clip = s.clip;
                musicSource.Play();
            }
        }
        
        
        public void PlaySfx(string name)
        {
            Sound s = Array.Find(sfxSounds, x => x.name == name);
            if (s == null)
            {
                Debug.Log("Sound Not Found");
            }
            else
            {
                Debug.Log("PlaySfx: " + name);
                sfxSource.PlayOneShot(s.clip);
            }
        }
    }

    

}