using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FTK_MultiMax_Rework {
    public static class DummiesHandler {

        public static void CreateDummies() {
            Debug.Log("Creating Dummies...");
            List<GameObject> dummies = new List<GameObject>();

            for (int j = 0; j < Mathf.Max(3, GameFlowMC.gMaxPlayers); j++) {
                dummies.Add(CreateDummy(FTKHub.Instance.m_Dummies, j, "Player"));
            }

            for (int i = 0; i < Mathf.Max(3, GameFlowMC.gMaxEnemies); i++) {
                dummies.Add(CreateDummy(FTKHub.Instance.m_Dummies, i + 3, "Enemy"));
            }

            FTKHub.Instance.m_Dummies = dummies.ToArray();
            Debug.Log("Dummies created!");
        }

        public static GameObject CreateDummy(GameObject[] source, int index, string prefix) {
            GameObject dummy;
            if (index < 3) {
                dummy = source[index];
            } else {
                dummy = UnityEngine.Object.Instantiate(source[2], source[2].transform.parent);
                dummy.name = $"{prefix} {index + 1} Dummy";
                dummy.GetComponent<PhotonView>().viewID = 3245 + index;
            }
            return dummy;
        }

        public static void DummySlide() {
            DummyAttackSlide[] array = UnityEngine.Object.FindObjectsOfType<DummyAttackSlide>();
            foreach (DummyAttackSlide dummyAttackSlide in array) {
                if (dummyAttackSlide.m_Distances.Length < 1000) {
                    float[] array2 = new float[1000];
                    Array.Copy(dummyAttackSlide.m_Distances, array2, dummyAttackSlide.m_Distances.Length);
                    dummyAttackSlide.m_Distances = array2;
                    Debug.Log(dummyAttackSlide.m_Distances);
                }
            }
        }
    }
}
