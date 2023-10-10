using DV;
using DV.UI;
using DV.UIFramework;
using RSG;
using System.Collections;

namespace MessageBox;

public static class PopupAPI
{

	public static IPromise<PopupResult> ShowOk(string message, string? title = null, string? positive = null)
	{
		return new Promise<PopupResult>((resolve, reject) => {
			ShowOk(message, (result) => resolve(result), title, positive);
		});
	}
	public static void ShowOk(string message, PopupClosedDelegate onClose, string? title = "", string? positive = "Ok")
	{
		// TODO: Should ShowPopupOk even accept a title argument? The prefab doesn't appear to display it.
		ShowPopup(uiReferences.popupOk, new PopupLocalizationKeys {
			titleKey = title,
			labelKey = message,
			positiveKey = positive
		}, onClose);
	}

	public static IPromise<PopupResult> ShowYesNo(string message, string? title = null, string? positive = null, string? negative = null)
	{
		return new Promise<PopupResult>((resolve, reject) => {
			ShowYesNo(message, (result) => resolve(result), title, positive, negative);
		});
	}
	public static void ShowYesNo(string message, PopupClosedDelegate onClose, string? title = "", string? positive = "Yes", string? negative = "No")
	{
		ShowPopup(uiReferences.popupYesNo, new PopupLocalizationKeys {
			titleKey = title,
			labelKey = message,
			positiveKey = positive,
			negativeKey = negative,
		}, onClose);
	}

	public static IPromise<PopupResult> Show3Buttons(string message, string? title = null, string? positive = null, string? negative = null, string? abort = null)
	{
		return new Promise<PopupResult>((resolve, reject) => {
			Show3Buttons(message, (result) => resolve(result), title, positive, negative, abort);
		});
	}
	public static void Show3Buttons(string message, PopupClosedDelegate onClose, string? title = "", string? positive = "Yes", string? negative = "No", string? abort = "Abort")
	{
		ShowPopup(uiReferences.popup3Buttons, new PopupLocalizationKeys {
			titleKey = title,
			labelKey = message,
			positiveKey = positive,
			negativeKey = negative,
			abortionKey = abort,
		}, onClose);
	}

	private static void ShowPopup(Popup prefab, PopupLocalizationKeys locKeys, PopupClosedDelegate? onClose)
	{
		if (WorldStreamingInit.IsLoaded)
		{
			CoroutineManager.Instance.Run(Coro(prefab, locKeys, onClose));
		}
		else
		{
			WorldStreamingInit.LoadingFinished += () => CoroutineManager.Instance.Run(Coro(prefab, locKeys, onClose));
		}
	}

	private static IEnumerator Coro(Popup prefab, PopupLocalizationKeys locKeys, PopupClosedDelegate? onClose)
	{
		while (AppUtil.Instance.IsTimePaused)
			yield return null;
		while (!PopupManager.CanShowPopup())
			yield return null;
		Popup popup = PopupManager.ShowPopup(prefab, locKeys, keepLiteralData: true);
		if (onClose != null) { popup.Closed += onClose; }
	}

	private static PopupManager PopupManager
	{
		get => ACanvasController<CanvasController.ElementType>.Instance.PopupManager;
	}

	private static PopupNotificationReferences uiReferences
	{
		get => ACanvasController<CanvasController.ElementType>.Instance.uiReferences;
	}
};
