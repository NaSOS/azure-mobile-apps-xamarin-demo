# GlobalAzureBootcamp2017

Azure Mobile Apps and Xamarin demo for Global Azure Bootcamp 2017 in Athens, Greece to showcase how to create an Azure Mobile App and setup the client SDK. The demo showcases:
- Offline Sync
- User Authentication
- Push notifications
- Key phrase extraction (Cognitive Services)

## Prerequisites

- An active Microsoft Azure Subscription
- Xamarin Studio or Microsoft Visual Studio

## Getting Started

Open the project with Xamarin Studio and have a first look at the code. You will notice that there are 3 projects:
- GlobalAzureBootcamp2017: the PCL project that contains the shared application code as long as the application UI (Xamarin.Forms)
- GlobalAzureBootcamp2017.iOS: the iOS target app
- GlobalAzureBootcamp2017.Droid: the Android target app

## Setup

This is a step-by-step guide to setup all the [Azure Mobile Apps](https://docs.microsoft.com/en-us/azure/app-service-mobile/app-service-mobile-value-prop) services that are required by the demo app to function properly.

### Azure Mobile Apps

Step zero, first you need to install the [Azure Mobile Client SDK](https://docs.microsoft.com/en-us/azure/app-service-mobile/app-service-mobile-dotnet-how-to-use-client-library):
- On the *Solution Explorer* of the *GlobalAzureBootcamp2017* project, select *Getting Started*
- Select *Add an Azure Mobile Backend*
- The dependencies are already installed on the demo but on new projects you need to install them
- Now you can select your *App Service* or create a new one (**You have to be logged in with your Microsoft account**)
- Once your App Service is live you should be able to see your App Service URL on the code templates below, copy it and update the `ApplicationUrl` constant

Your Azure Mobile Backend is now ready!

#### Offline Sync (Easy Tables)

Step one, [Offline Sync](https://docs.microsoft.com/en-us/azure/app-service-mobile/app-service-mobile-xamarin-forms-get-started-offline-data). Before being able to upload data to Azure Mobile Backend you need to create the required tables. To do that:
- Login to *Azure Portal* and select the Mobile App Service that you created.
- Search for *Easy Tables*
- First you need to configure *Easy Tables*:
	- 1. Add a new *Data Connection*
	- 2. Initialize your *App Service* to use *Easy Tables*
- Now you can create the required tables. Press *Add* and create 3 tables: `Event`, `Activity` and `Speaker` (do not set any permissions)
	- Event: the event, on our case the Global Azure Bootcamp 2017, Athens.
	- Activity: The scheduled activities of the Event
	- Speaker: The info of a Speaker (if the `ActivityType` is a `Talk`)

**Note:** Each table has the default columns: `id`, `createdAt`, `updatedAt`, `version` and `deleted`. There is no need to add any more columns as they will be created automatically upon data insertion.

Now you can populate the tables with the initial data. This can be done through the app by uncommenting the `#define MIGRATION_ENABLED` on the `AzureService` class. By doing so you enable the execution of the `Migrate()` method that reads the Global Azure Bootcamp's schedule from a `JSON` formatted string and pushed the data to the backend. Don't forget to comment out that line again upon completion because we don't want this code to run on every app launch.

**NOTE:** Now your *Easy Tables* will have the required data. Notice that each table's schema is updated and includes the additional properties of it's respective model.

##### Important
Although you can add multiple Events the demo works only for one and thus, you need to replace the `EventId` constant with the id of the event entry that was created during data insertion!

#### User Authentication

Step two, it's time to setup [User Authentication](https://docs.microsoft.com/en-us/azure/app-service-mobile/app-service-mobile-xamarin-forms-get-started-users):
- On *Azure Portal*, select your *Mobile App Service*
- Search for *Authentication / Authorization*
- Enable *App Service Authentication*

Now you will be able to see the list of the available *Authentication Providers*. Let's enable one (in our case Microsoft):
- Select *Microsoft*
	- Follow [this](https://docs.microsoft.com/en-us/azure/app-service-mobile/app-service-mobile-how-to-configure-microsoft-authentication) for instructions on how to configure the appropriate Microsoft app
	- Note the *AppId* and the *Application Secret*
- Fill in the *Client Id* and *Client Secret* fields with *AppId* and *Application Secret* respectively
- Leave the *Action to take when request is not authenticated* to *Allow Anonymous requests (no action)* because on our demo, for convenience, we are not going to restrict all tables

Now you can enable authentication on the *Easy Tables* to restrict access to authorised users only. To do so:
- Go back to *Easy Tables*
- Create the table `UserActivity` and set all permissions to *Authenticated access only*

That's it, now only authenticated users will have access to `UserActivity` table! 

**NOTE:** On the demo we use this table to store the event activities that a user bookmark's.

#### Push notifications

Step three, [Push Notifications](https://docs.microsoft.com/en-us/azure/app-service-mobile/app-service-mobile-xamarin-forms-get-started-push):
- On *Azure Portal*, select your *Mobile App Service*
- Search for *Push* and press *Connect*
	- Select or create a *Notification Hub*
- Once your app is connected, press *Configure push notification services*
- Select the service you want to configure (on our Demo Google)
	- Follow [this](https://docs.microsoft.com/en-us/azure/app-service-mobile/app-service-mobile-xamarin-forms-get-started-push) for instructions on how to configure *Cloud Messaging* on *Firebase*
- Fill in the *API Key* with the Firebase *Server key*
- Update the `GCMSenderId` constant of the demo app with the *Sender ID*

Your app can now receive push notifications! Let's add some server-side logic in order to send push notifications to the app.
- Go back to *Easy Tables*
- Create the `EventUpdate` table (do not set any permissions)
- Select the table and then press *Edit script*
- A new window will open with the server-side code of the table. Replace it with the code below.
	- With this code, on every insert operation a push notification will be sent to all devices
	- The notification message will be the `message` property of the inserted item

```javascript
var azureMobileApps = require('azure-mobile-apps'),
promises = require('azure-mobile-apps/src/utilities/promises'),
logger = require('azure-mobile-apps/src/logger');

var table = azureMobileApps.table();

table.insert(function (context) {
	// For more information about the Notification Hubs JavaScript SDK,
	// see http://aka.ms/nodejshubs
	logger.info('Running insert');
	
	// Define the template payload.
	var payload = '{"messageParam": "' + context.item.message + '" }';  
	
	// Execute the insert.  The insert returns the results as a Promise,
	// Do the push as a post-execute action within the promise flow.
	return context.execute()
		.then(function (results) {
				// Only do the push if configured
				if (context.push) {
					// Send a template notification.
					context.push.send(null, payload, function (error) {
						if (error) {
							logger.error('Error while sending push notification: ', error);
						} else {
							logger.info('Push notification sent successfully!');
						}
					});
				}
				// Don't forget to return the results from the context.execute()
				return results;
		})
		.catch(function (error) {
			logger.error('Error while running context.execute: ', error);
		});
});

module.exports = table;
```

### Cognitive Services

Step four, let's add some AI to our app. This service is not within the scope of *Azure Mobile Apps* but will make the demo more interesting! We'll use the [Text Analytics API](https://azure.microsoft.com/en-us/services/cognitive-services/text-analytics/) and specifically the *Key phrase extraction* feature in order to highlight the key phrases of the description of an Activity:

- On *Azure Portal*, select *New*
- Search for *Cognitive Services*
- Create a new service and select *Text Analytics API* as *API Type*
- Select your *Cognitive Service* and search for *Keys*
- Replace the `CognitiveServicesKey` constant with any of the given keys

In order to see how the service works, you can execute the below request (replace the `COGNITIVE_SERVICE_KEY` with your key):
```
curl -X POST \
  https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/keyPhrases \
  -H 'accept: application/json' \
  -H 'cache-control: no-cache' \
  -H 'content-type: application/json' \
  -H 'ocp-apim-subscription-key: {COGNITIVE_SERVICE_KEY}' \
  -d '{
     "documents": [
         {
             "language": "en",
             "id": "1",
             "text": "Mobile Apps in Azure App Service offer a highly scalable, globally available mobile application development platform that brings a rich set of capabilities to mobile developers. With Mobile Apps it’s easy to rapidly build engaging cross-platform and native apps for iOS, Android, Windows, or Mac; store app data in the cloud or on-premises with offline syncing functionality; authenticate users by selecting from an ever-growing list of identity providers; send push notifications; or add your custom backend logic in C# or Node.js. This presentation demonstrates how to get started with Azure Mobile Apps and easily build an Azure backed cross-platform mobile application."
         }
     ]
 }'
```

### Mobile Center

Step five, in order to monitor our app's activity we'll enable [Mobile Center](https://docs.microsoft.com/en-us/mobile-center/):
- Login to [Mobile Center Console](https://mobile.azure.com/apps)
- Create an iOS app and replace the `MobileCenterIosKey` constant with the given key
- Create an Android app and replace the `MobileCenterAndroidKey` constant with the given key

## Test

Before testing the demo make sure you have updated all the below constants during setup:
- `ApplicationUrl`
- `EventId`
- `GCMSenderId`
- `CognitiveServicesKey`
- `MobileCenterAndroidKey`
- `MobileCenterIosKey`

If so, you are finally ready to run the demo app:
- The demo currently shows only one Event
- The Event's Activities are automatically grouped by their place
- *My Schedule* tab contains the bookmarked Activities, `UserActivities` (needs authentication)
- By opening an Activity you can see the respective info and bookmark it (needs authentication)
- Finally, the *Updates* screen shows the `EventUpdates`. Remember that this table was created during the *Push Notification* setup? In order to add a new entry and test the push notification mechanism you can run the below request:
```
curl -X POST \
  {APP_SERVICE_URL}/tables/eventupdate \
  -H 'cache-control: no-cache' \
  -H 'content-type: application/json' \
  -H 'zumo-api-version: 2.0.0' \
  -d '{
  "eventId": "{EVENT_ID}",
  "message": "Xamarin rules!"
}'
```
