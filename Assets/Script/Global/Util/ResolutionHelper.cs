

using UnityEngine;
using UnityEngine.UI;

namespace AdverGame.Utility
{
    public static class ResolutionHelper
    {
        static Vector2 m_referenceResolution = Vector2.zero;
        static Vector2 m_currentResolution = Vector2.zero;
        public static void ScaleToFitScreen(GameObject obj)
        {
            var rectObj = obj.GetComponent<RectTransform>();
            if (m_referenceResolution == Vector2.zero || m_currentResolution == Vector2.zero)
            {
                var mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
                m_currentResolution = mainCanvas.GetComponent<RectTransform>().sizeDelta;
                m_referenceResolution = mainCanvas.GetComponent<CanvasScaler>().referenceResolution;

            }
            var diffHeigthr = (m_referenceResolution.y - rectObj.sizeDelta.y) ;
            var diffHeigthc = (m_currentResolution.y - rectObj.sizeDelta.y);
            Debug.Log(diffHeigthr);
            Debug.Log(diffHeigthc);
            var dif = new Vector2((m_currentResolution.x - m_referenceResolution.x) + rectObj.sizeDelta.x, rectObj.sizeDelta.y);

            rectObj.sizeDelta = dif;
        }


    }
}
