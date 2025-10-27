using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Cheats : MonoBehaviour
{
    [SerializeField] private InputActionReference _restartAction;

    private void OnEnable()
    {
        _restartAction.action.Enable();
        
        _restartAction.action.performed += OnRestartPerformed;
    }

    private void OnDisable()
    {
        _restartAction.action.performed -= OnRestartPerformed;
        _restartAction.action.Disable();
    }
    
    private void OnRestartPerformed(InputAction.CallbackContext obj)
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
