using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FTK_MultiMax_Rework {
    public static class DummiesHandler {

        public static void CreateDummies() {
            Debug.Log("MAKING DUMMIES");
            List<GameObject> dummies = new List<GameObject>();
            for (int j = 0; j < Mathf.Max(3, GameFlowMC.gMaxPlayers); j++) {
                if (j < 3) {
                    dummies.Add(FTKHub.Instance.m_Dummies[j]);
                    continue;
                }
                GameObject copy2 = UnityEngine.Object.Instantiate(FTKHub.Instance.m_Dummies[2], FTKHub.Instance.m_Dummies[2].transform.parent);
                copy2.name = "Player " + (j + 1) + " Dummy";
                copy2.GetComponent<PhotonView>().viewID = 3245 + j;
                dummies.Add(copy2);
            }
            for (int i = 0; i < Mathf.Max(3, GameFlowMC.gMaxEnemies); i++) {
                if (i < 3) {
                    dummies.Add(FTKHub.Instance.m_Dummies[i + 3]);
                    continue;
                }
                GameObject copy = UnityEngine.Object.Instantiate(FTKHub.Instance.m_Dummies[5], FTKHub.Instance.m_Dummies[5].transform.parent);
                copy.name = "Enemy " + (i + 1) + " Dummy";
                copy.GetComponent<PhotonView>().viewID = 3045 + i;
                dummies.Add(copy);
            }
            FTKHub.Instance.m_Dummies = dummies.ToArray();
            GameObject[] dummies2 = FTKHub.Instance.m_Dummies;
            foreach (GameObject go in dummies2) {
                Debug.Log("DUMMY");
                Debug.Log(go.name);
            }
            Debug.Log("MultiMax - Done");
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
