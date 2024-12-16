using Assist;
using UnityEngine;

namespace panelFunction
{
    public class SetPanelFunc : MonoBehaviour
    {
        public void OpenDebugPanel()
        {
            AudioManager.Instance.PlaySfx("btn_operate");
            PanelManager.Open<DebugPanel>();
        }
        
        public void OpenSerialPanel()
        {
            AudioManager.Instance.PlaySfx("btn_operate");
            PanelManager.Open<SerialPanel>();
        }
    }
}