using UnityEngine;
using System.Collections.Generic;

public class VoiceOverManager : MonoBehaviour
{
    [SerializeField] private List<AudioClip> VCList;
    [SerializeField] public AudioSource VCSource;
    public Queue<AudioClip> VCQueue;
    public bool Finished;



    private void Awake()
    {
        if (VCSource == null)
        {
            VCSource = GetComponentInChildren<AudioSource>();
        }

        VCQueue = new Queue<AudioClip>();


        foreach (AudioClip audio in VCList)
        {
            VCQueue.Enqueue(audio);
        }

        Finished = false;
    }

    public void PlayClip()
    {
        if (VCQueue.Count != 0)
        {
            VCSource.clip = VCQueue.Dequeue();
            if (VCSource.clip != null)
            {
                Debug.Log($"[VoiceOverManager] - clip name - {VCSource.clip.name}");
                VCSource.Play();
            }
            else
            {
                Debug.Log($"[VoiceOverManager] - hit end");
            }
        }
        else
        {
            Finished = true;
        }
    }

}
