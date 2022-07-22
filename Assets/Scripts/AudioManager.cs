using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeenieWalker
{
    public class AudioManager : MonoSingleton<AudioManager>
    {

        [SerializeField] List<AudioSource> sources = new List<AudioSource>();
        [SerializeField] List<AudioClip> voClips = new List<AudioClip>();
        [SerializeField] List<AudioClip> questionAnswerClips = new List<AudioClip>();
        [SerializeField] float mimicWait = 1f;

        private void OnEnable()
        {
            LootChest.OnLootChestPicked += PlayVOClip;
        }

        private void OnDisable()
        {
            LootChest.OnLootChestPicked -= PlayVOClip;
        }

        private void PlayVOClip(bool isMimic)
        {
            if (voClips.Count < 1)
                return;

            if (isMimic)
            {
                int randClip = Random.Range(0, voClips.Count);
                AudioClip clip = voClips[randClip];

                sources[0].clip = clip;
                sources[0].PlayDelayed(mimicWait);
            }
        }

        public void PlayVOClip(int clipNum)
        {

            AudioClip clip = voClips[clipNum];

            sources[0].clip = clip;
            sources[0].Play();
        }

        public void PlayAnsweredQuestionClip(bool isCorrect)
        {
            int correct = isCorrect ? 1:0;

            sources[2].clip = questionAnswerClips[correct];
            sources[2].Play();
        }
    }

    public enum AudioSourceOptions
    {
        VO,
        BGM,
        SFX
    }
}
