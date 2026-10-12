using MergeGame.Loading;
using UnityEngine;

namespace MergeGame.Menu
{
    public class MenuController : MonoBehaviour
    {
        public void Play()
        {
            SceneLoader.Load(Constants.SceneNames.Game);
        }
    }
}
