 - To run this project
	1. Copy the vmi.apk file to your android device (Android 2.3.1 newer)
	2. Install the app
	3. Open and enjoy it.

 - To compile the project
	1. Open unity and open the directory where you unzipped the project
	2. Build as android device
	3. Open Player Setting, Set API compatible level as .Net 2.0 (NOT .Net 2.0 Subset)
	4. Build and Run~ Enjoy

 - Tips if you want to modify the project
	1. The KeyManage.cs must be attached to Main Camera as only Main Camera has default Audio Source
	2. The sensor fusion code is written in Java, called by Unity AndroidJNI. The AndroidJNI file is borrowed from Unity 3.x which name is classes.jar under the folder Assets/Plugins. Don't remove it.
	3. To compile java file, please go to the Assets/Plugins/Android/src and find compile.bat which is a template bat file to help you compile java code to jar for unity.
	
 - Demos
	1. Canon Chords: https://www.youtube.com/watch?v=VUSNSaQnGHw
	2. Are you sleeping: https://www.youtube.com/watch?v=BClJr9B7xPw
 