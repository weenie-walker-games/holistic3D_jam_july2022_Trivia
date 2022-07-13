using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WeenieWalker
{
    public class LootChest : MonoBehaviour, IPointerClickHandler
    {

        public static event System.Action<bool> OnLootChestPicked;

        [SerializeField] GameObject[] chests = new GameObject[2];
        [SerializeField] Animator lootAnimator;
        [SerializeField] Animator mimicAnimator;
        [SerializeField] AudioSource correctAudio;
        private bool isMimic = false;
        public bool IsMimic { get { return isMimic; } set
            {
                isMimic = value;

                if (isMimic)
                {

                    chests[0].SetActive(false);
                    chests[1].SetActive(true);
                    mimicAnimator.SetTrigger("shut");

                }
                else
                {
                    chests[0].SetActive(true);
                    chests[1].SetActive(false);
                }
            } }


        private void OnEnable()
        {
         
        }

        private void OnDisable()
        {
            
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(IsMimic)
            {
                mimicAnimator.SetTrigger("attack1");
            }
            else
            {
                lootAnimator.SetTrigger("openLid");
                correctAudio.Play();
                Invoke("CloseLid", 4f);
            }

            OnLootChestPicked?.Invoke(isMimic);
        }


        private void CloseLid()
        {
            lootAnimator.SetTrigger("closeLid");
            correctAudio.Stop();
        }
    }
}
