using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Puzzle03 : MonoBehaviour
{
    private Camera _cam;
    [SerializeField] private List<GameObject> books = new List<GameObject>();
    [SerializeField] private List<GameObject> bookPlace = new List<GameObject>();
    [SerializeField] private Transform hand;
    private bool _emptyHand = true;
    [SerializeField] private GameObject door;
    [SerializeField]private int _doorCount = 0;
    private GameObject _hitObj;
    [SerializeField] private Animator doorOpen;
    private bool _launchFinal = false;
    [SerializeField] private Transform finalCamLocation;
    [SerializeField] private Animator ending;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameObject pointer;
    [SerializeField] private GameObject credit;
    [SerializeField] private GameObject qrCode;
    [SerializeField] private GameObject player;
    
    void Start()
    {
        _cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out hit, 1.75f))
            {
                GameObject hitObj = hit.collider.gameObject;

                if (_emptyHand && books.Contains(hitObj))
                {
                    Rigidbody rb = hitObj.GetComponent<Rigidbody>();
                    if (!rb) return;

                    _hitObj = hitObj;
                    rb.isKinematic = true;
                    _emptyHand = false;

                    Debug.Log("Book grabbed");
                }

                else if (!_emptyHand && hitObj == door)
                {
                    int index = books.IndexOf(_hitObj);

                    if (index < 0 || index >= bookPlace.Count)
                    {
                        Debug.LogWarning("No matching book place found!");
                        return;
                    }

                    Transform targetPlace = bookPlace[index].transform;

                    _hitObj.transform.position = targetPlace.position;
                    _hitObj.transform.rotation = targetPlace.rotation;

                    Rigidbody rb = _hitObj.GetComponent<Rigidbody>();
                    if (rb) rb.isKinematic = true;

                    books.RemoveAt(index);
                    bookPlace.RemoveAt(index);

                    _hitObj = null;
                    _emptyHand = true;
                    _doorCount++;

                    Debug.Log("Book placed in correct slot");
                }
            }
        }
        if (!_emptyHand && _hitObj != null)
        {
            _hitObj.transform.position = hand.position;
            _hitObj.transform.rotation = hand.rotation;
        }

        if (_doorCount == 3 && !_launchFinal)
            StartCoroutine(FinalLerp(finalCamLocation));
        
    }

    private IEnumerator FinalLerp(Transform target, float duration = 2f)
    {
        if (playerMovement)
            playerMovement.enabled = false;
        
        player.SetActive(false);
        
        if(qrCode)
            qrCode.SetActive(false);
        
        pointer.SetActive(false);
        _cam.transform.SetParent(null);
        
        Vector3 startPos = _cam.transform.position;
        Quaternion startRot = _cam.transform.rotation;
        
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            t = Mathf.SmoothStep(0f, 1f, t);

            _cam.transform.position = Vector3.Lerp(startPos, target.position, t);
            _cam.transform.rotation = Quaternion.Slerp(startRot, target.rotation, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        _cam.transform.position = target.position;
        _cam.transform.rotation = target.rotation;
        
        doorOpen.enabled = true;
        yield return new WaitForSeconds(2f);
        ending.enabled = true;
        yield return new WaitForSeconds(1f);
        credit.SetActive(true);
        
        _launchFinal = true;
    }
}

