using System;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] private List<AudioClip> SFXList;
    [SerializeField] public AudioSource SFXSource;
    public Queue<AudioClip> SFXQueue;
    public bool Finished;


    private void Awake()
    {
        if (SFXSource == null)
        {
            SFXSource = GetComponentInChildren<AudioSource>();
        }
        SFXQueue = new Queue<AudioClip>();

        foreach (AudioClip audio in SFXList)
        {
            SFXQueue.Enqueue(audio);
        }

        Finished = false;
    }


    public void PlayClip()
    {
        if (SFXQueue.Count != 0)
        {
            SFXSource.clip = SFXQueue.Dequeue();

            if (SFXSource.clip != null)
            {
                Debug.Log($"[SFXSourceManager] - clip name - {SFXSource.clip.name}");
                SFXSource.Play();
            }
            else
            {
                Debug.Log($"[SFXSourceManager] - hit end");
            }
        }
        else
        {
            Finished = true;
        }
    }


}
