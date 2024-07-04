using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimatedButtonClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	private RectTransform _rectTransform = null;

	[SerializeField]
	private float scaleSpeed = 0.3f;
	[SerializeField]
	private Vector3 maxSize = Vector3.one * 0.1f;

	private void Awake()
	{
		_rectTransform = GetComponent<RectTransform>();
	}

	public IEnumerator Execute(bool isDown)
	{
		float time = 0f;
		Vector3 currentScale = _rectTransform.localScale;
		Vector3 targetScale = isDown ? maxSize : Vector3.one;
		while (_rectTransform.localScale != targetScale)
		{
			time += Time.deltaTime;
			Vector3 scale = Vector3.Lerp(currentScale, targetScale, time / scaleSpeed);
			_rectTransform.localScale = scale;
			yield return null;
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		StopAllCoroutines();
		StartCoroutine(Execute(false));
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		StopAllCoroutines();
		StartCoroutine(Execute(true));
	}
}
