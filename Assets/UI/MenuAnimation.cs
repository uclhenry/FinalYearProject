using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class MenuAnimation : MonoBehaviour {
	
	public Animator menuAnimation;
	//    public Animator settingsButton;
	public Text text;
	public GameObject news;
	public GameObject whatsaround;


	
//	private int eventState = 0; // 1 is down 0 is up
	// Use this for initialization
//	void Awake () {
//		menuAnimation.SetBool ("ActivateMenu", false);	
//	}
	
//	// Update is called once per frame
//	public void ActivateMenuDownFromUppermenu () {
//		
//		
//		//    menuAnimation.SetBool ("ActivateMenu", false);
//		
//		
//		switch(eventState)
//		{
//		case 0: 
//			Debug.Log("UP");
//			menuAnimation.SetBool ("ActivateMenu", false);
//			eventState = 1;
//			break;
//		case 1:
//			Debug.Log("Down");
//			menuAnimation.SetBool ("ActivateMenu", true);
//			eventState = 0;
//			break;
//		default:
//			Debug.Log("NOTHING");
//			break;
//		}
//		
//		
//		
//		
//	}
//	
	
	
	
	
	
	public void ActivateMenuClose () {
		
		menuAnimation.SetBool ("ActivateMenu", true);
//		eventState = 1;
		
		
		
	}
	
	public void ActivateMenuOpen () {
//		eventState = 0;
		menuAnimation.SetBool ("ActivateMenu", false);
		
	}
	
	public void News () {
		

		menuAnimation.SetBool("ActivateMenu", true);
		whatsaround.gameObject.SetActive (false);

		news.gameObject.SetActive (true);

		
	}

	public void WhatAround () {
		
		
		menuAnimation.SetBool("ActivateMenu", true);
		whatsaround.gameObject.SetActive (true);
		news.gameObject.SetActive (false);

		
	}

	public void AboutUs () {
		
		
		menuAnimation.SetBool("ActivateMenu", true);
		
		
	}

	public void Routes () {
		
		
		menuAnimation.SetBool("ActivateMenu", true);
		
		
	}

	public void OnDrag() { 
		transform.position = Input.mousePosition; 
	}


	//	
//	public void ActivateMenuUpAboutUs () {
//		eventState = 0;
//		menuAnimation.SetBool("ActivateMenu", true);
//		text.text = "ABOUT US";
//		StartCoroutine(LoadAboutUsPage());
//	}
//	
//	public void ActivateMenuUpHighlights () {
//		eventState = 0;
//		menuAnimation.SetBool("ActivateMenu", true);
//		text.text = "HIGHLIGHTS";
//		
//		StartCoroutine(LoadHighlightsPage());
//		
//	}
//	
//	public void ActivateMenuUpStreetStyle () {
//		eventState = 0;
//		menuAnimation.SetBool("ActivateMenu", true);
//		text.text = "STREET STYLE";
//		StartCoroutine(LoadStreetStyle());
//		
//	}
//	
//	public void ActivateMenuUpCelebrety () {
//		eventState = 0;
//		menuAnimation.SetBool("ActivateMenu", true);
//		text.text = "CELEBRITY";
//		StartCoroutine(LoadCelebrity());
//		
//	}
//	
//	public void ActivateMenuUpLFW () {
//		eventState = 0;
//		menuAnimation.SetBool("ActivateMenu", true);
//		text.text = "LFW TV";
//		StartCoroutine(LoadLFWTV());
//		
//	}
//	
//	public void ActivateMenuUpShowSchedules () {
//		eventState = 0;
//		menuAnimation.SetBool("ActivateMenu", true);
//		text.text = "SHOW SCHEDULES";
//		StartCoroutine(LoadSchedules());
//		
//	}
//	
//	public void ActivateMenuUpDesigners () {
//		eventState = 0;
//		menuAnimation.SetBool("ActivateMenu", true);
//		text.text = "DESIGNERS";
//		StartCoroutine(LoadDesigners());
//	}
//	public void ActivateMenuUpGallery () {
//		eventState = 0;
//		menuAnimation.SetBool("ActivateMenu", true);
//		text.text = "GALLERY";
//		StartCoroutine(LoadGalleryPage());
//		
//		
//	}
//	
//	
//	
//	
//	public IEnumerator LoadHighlightsPage()
//	{
//		//        yield return new WaitForSeconds(animation[MenuOpenFinal].Length);
//		yield return new WaitForSeconds(0.5f);
//		Application.LoadLevel ("Highlights");    
//	}
//	
//	
//	public IEnumerator LoadGalleryPage()
//	{
//		//        yield return new WaitForSeconds(animation[MenuOpenFinal].Length);
//		yield return new WaitForSeconds(0.5f);
//		Application.LoadLevel ("GalleryPage");    
//	}
//	
//	public IEnumerator LoadLFWTV()
//	{
//		//        yield return new WaitForSeconds(animation[MenuOpenFinal].Length);
//		yield return new WaitForSeconds(0.5f);
//		Application.LoadLevel ("LFWTV");    
//	}
//	
//	public IEnumerator LoadDesigners()
//	{
//		//        yield return new WaitForSeconds(animation[MenuOpenFinal].Length);
//		yield return new WaitForSeconds(0.5f);
//		Application.LoadLevel ("Designers");    
//	}
//	
//	public IEnumerator LoadHomePage()
//	{
//		//        yield return new WaitForSeconds(animation[MenuOpenFinal].Length);
//		yield return new WaitForSeconds(0.5f);
//		Application.LoadLevel ("HomePage");
//	}
//	
//	public IEnumerator LoadAboutUsPage()
//	{
//		//        yield return new WaitForSeconds(animation[MenuOpenFinal].Length);
//		yield return new WaitForSeconds(0.5f);
//		Application.LoadLevel ("AboutUs");
//	}
//	public IEnumerator LoadSchedules()
//	{
//		//        yield return new WaitForSeconds(animation[MenuOpenFinal].Length);
//		yield return new WaitForSeconds(0.5f);
//		Application.LoadLevel ("ShowSchedules");
//	}
//	public IEnumerator LoadCelebrity()
//	{
//		//        yield return new WaitForSeconds(animation[MenuOpenFinal].Length);
//		yield return new WaitForSeconds(0.5f);
//		Application.LoadLevel ("Celebrity");
//	}
//	public IEnumerator LoadStreetStyle()
//	{
//		//        yield return new WaitForSeconds(animation[MenuOpenFinal].Length);
//		yield return new WaitForSeconds(0.5f);
//		Application.LoadLevel ("StreetStyle");
//	}
//	
	
	
}

