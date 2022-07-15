using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeenieWalker
{
    public class AudioManager : MonoSingleton<AudioManager>
    {

        [SerializeField] AudioSource source;
        [SerializeField] List<AudioClip> voClips;
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

                source.clip = clip;
                source.PlayDelayed(mimicWait);
            }
        }

    }
}
