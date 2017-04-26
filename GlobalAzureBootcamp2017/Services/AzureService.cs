// Uncomment the MIGRATION_ENABLED to upload your initial data. Then comment it out again.
//#define MIGRATION_ENABLED

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;

#if MIGRATION_ENABLED
using System.Diagnostics;
using Newtonsoft.Json;
#endif

namespace GlobalAzureBootcamp2017
{
	public class AzureService : IService
	{
		#region Singleton
		public static AzureService Instance { get; } = new AzureService();
		#endregion

		#region Properties
		public MobileServiceClient Client { get; private set; }

		public EventRepository EventRepository { get; private set; }

        public EventUpdateRepository EventUpdateRepository { get; private set; }

		public SpeakerRepository SpeakerRepository { get; private set; }

		public ActivityRepository ActivityRepository { get; private set; }

		public UserActivityRepository UserActivityRepository { get; private set; }

        public TextAnalyticsRepository TextAnalyticsRepository { get; private set; }
		#endregion

		#region Constructor
		private AzureService()
		{
			var url = new Uri(Constants.ApplicationUrl);

			// init service                                   
			Client = new MobileServiceClient(url);

			// init local store
			var store = new MobileServiceSQLiteStore($"{url.Host}.db");
			store.DefineTable<Event>();
            store.DefineTable<EventUpdate>();
			store.DefineTable<Speaker>();
			store.DefineTable<Activity>();
			store.DefineTable<UserActivity>();

			//Initializes the SyncContext using the default IMobileServiceSyncHandler.
			Client.SyncContext.InitializeAsync(store);

			// init repositories
            EventRepository = new EventRepository(this, Client.GetSyncTable<Event>());
            SpeakerRepository = new SpeakerRepository(this, Client.GetSyncTable<Speaker>());
            ActivityRepository = new ActivityRepository(this, Client.GetSyncTable<Activity>());
            UserActivityRepository = new UserActivityRepository(this, Client.GetSyncTable<UserActivity>(), ActivityRepository);
            EventUpdateRepository = new EventUpdateRepository(this, Client.GetSyncTable<EventUpdate>());

            TextAnalyticsRepository = new TextAnalyticsRepository();

#if MIGRATION_ENABLED
			Task.Run(async () =>
			{
				await Migrate();
			});
#endif

		}
		#endregion

		public async Task<bool> SyncAsync()
		{
			var syncTasks = new List<Task<bool>>
			{
				EventRepository.SyncAsync(),
                EventUpdateRepository.SyncAsync(),
				SpeakerRepository.SyncAsync(),
				ActivityRepository.SyncAsync()
			};

			// tables that require authentication
			if(AuthenticationHelper.Instance.IsAuthenticated)
			{
				syncTasks.Add(UserActivityRepository.SyncAsync());
			}

			var result = await Task.WhenAll(syncTasks).ConfigureAwait(false);
			return result.All(x => x);
		}

		public async Task PushAsync()
		{
			await Client.SyncContext.PushAsync();
		}

#if MIGRATION_ENABLED
		/// <summary>
		/// Method to upload the initial data.
		/// Don't forget to replace the Constants.EventId with the new eventId!!
		/// </summary>
		/// <returns>The migrate.</returns>
		public async Task Migrate()
		{
			Debug.WriteLine($"Migrate()");
			try
			{
				Debug.WriteLine($"Init auto mapper");
				// init auto mapper
				AutoMapper.Mapper.Initialize(cfg =>
				{
					cfg.CreateMap<EventDto, Event>();
					cfg.CreateMap<ActivityDto, Activity>();
					cfg.CreateMap<SpeakerDto, Speaker>().ForMember(dest => dest.Socials, opt => opt.MapFrom(src => string.Join(",", src.Socials)));
				});

				Debug.WriteLine($"Syncing data");

				// sync everything
				if (!(await SyncAsync())) return;

				// define tables
				var eventTable = Client.GetSyncTable<Event>();
				var activityTable = Client.GetSyncTable<Activity>();
				var speakerTable = Client.GetSyncTable<Speaker>();

				Debug.WriteLine($"Delete old data");

				// delete old data
				foreach(var e in await eventTable.ToEnumerableAsync()){
					await eventTable.DeleteAsync(e);
				}
				foreach (var a in await activityTable.ToEnumerableAsync())
				{
					await activityTable.DeleteAsync(a);
				}
				foreach (var s in await speakerTable.ToEnumerableAsync())
				{
					await speakerTable.DeleteAsync(s);
				}

				Debug.WriteLine($"Parse new data");

                // parse data
                var data = JsonConvert.DeserializeObject<EventDto>("{\"name\":\"Global Azure Bootcamp 2017 Athens\",\"date\":\"2017-04-22\",\"info\":\"Welcome to Global Azure Bootcamp! Around the world user groups and communities want to learn about Azure and Cloud Computing! On April 22, 2017, all communities will come together once again in the fifth great Global Azure Bootcamp event! Each user group will organize their own one day deep dive class on Azure the way they see fit and how it works for their members. The result is that thousands of people get to learn about Azure and join together online under the social hashtag #GlobalAzure! Join hundreds of other organizers to help out and be part of the experience!\",\"activities\":[{\"name\":\"Registration\",\"from\":\"2017-04-22 09:00\",\"type\":1,\"place\":\"Auditorium 1\"},{\"name\":\"Azure Service Fabric: Microservices Architecture made simple\",\"info\":\"Microservice architecture is a method of developing software applications as a suite of independently deployable, small, modular services in which each service runs a unique process and communicates through a well-defined, lightweight mechanism to serve a business goal.\\nIn this talk we will be introduced to Azure Service Fabric, a distributed systems platform that makes it easy to package, deploy and manage scalable and reliable applications composed of microservices.\",\"from\":\"2017-04-22 09:30\",\"to\":\"2017-04-22 10:30\",\"type\":2,\"place\":\"Auditorium 1\",\"speaker\":{\"name\":\"PARIS POLYZOS\",\"title\":\"Senior Software Engineer / Microsoft Azure MVP\",\"info\":\"Paris is a Senior Software Engineer and a Microsoft Azure MVP. He holds an MSc in Electical & Computer Engineering; he is passionate about the Web, Microsoft Technologies and the cloud!\",\"socials\":[\"https://ppolyzos.com/\",\"https://www.facebook.com/ppolyzos\",\"https://twitter.com/ppolyzos\",\"https://linkedin.com/in/ppolyzos\",\"https://github.com/ppolyzos\"]}},{\"name\":\"Azure Container Service\",\"info\":\"Container technologies are changing the way apps are built, deployed and run. In particular, Docker has grabbed the attention of the industry by simplifying the building and packaging of container apps so they can run anywhere, while bringing added benefits of agility and density. Come to this session and learn how Azure Container Service can make Container Management boring and can help you deploy, scale, and orchestrate container-based solutions.\",\"from\":\"2017-04-22 10:30\",\"to\":\"2017-04-22 11:30\",\"type\":2,\"place\":\"Auditorium 1\",\"speaker\":{\"name\":\"KOSTAS PANTOS\",\"title\":\"Cloud Solutions Architect @ Microsoft\",\"info\":\"Since 2001, when I started my professional career, I was given the opportunity to work with some of the largest Greek Software development Companies and worked with cutting edge Microsoft technologies. Nowadays I work as a Solutions Architect for Microsoft where I focus in providing innovative cloud solutions.\",\"socials\":[\"https://www.facebook.com/konstantinos.pantos\",\"https://twitter.com/kpantos\",\"https://www.linkedin.com/in/konstantinospantos\"]}},{\"name\":\"Break\",\"from\":\"2017-04-22 11:30\",\"to\":\"2017-04-22 11:45\",\"type\":3,\"place\":\"Auditorium 1\"},{\"name\":\"Design for scalability and High Availability on Microsoft Azure\",\"info\":\"This session focuses on practical architectural guidance for designing highly available, enterprise grade, infrastructure for your applications, using Microsoft Azure (IaaS). Topics include the design of highly available applications in the cloud, increasing the availability of existing applications, and design changes needed for applications that have been through a lift & shift migration.\",\"from\":\"2017-04-22 11:45\",\"to\":\"2017-04-22 12:45\",\"type\":2,\"place\":\"Auditorium 1\",\"speaker\":{\"name\":\"VAGGELIS KAPPAS\",\"title\":\"Premier Field Engineer for Virtualization and Azure IaaS @ Microsoft\",\"info\":\"IT Solutions expert and Systems Engineer with over 15 years of experience combining team leading and managerial skills. Frequent speaker on technology events, organized by IT Pro community and Microsoft about Windows Server, Hyper-V, Azure, Office 365\",\"socials\":[\"http://www.autoexec.gr/\",\"https://www.facebook.com/vaggelis.kappas.37\",\"https://www.linkedin.com/in/vkappas\"]}},{\"name\":\"Hybrid IT - Hybrid Datacenter\",\"info\":\"Hybrid Datacenter model is the trend that the analyst believe will be the most preferable option for the next 5 years. Are we ready to adopt this trend and what Microsoft Azure offers in order to deliver the efficiency, in our On Premises Datacenter. Can we change our mindset on our daily datacenter operations and integrate Cloud Features on our IT journey. In this session we will discover the Azure Services that can be delivered with a Hybrid configuration and how we can connect the two worlds to achieve more.\",\"from\":\"2017-04-22 12:45\",\"to\":\"2017-04-22 13:45\",\"type\":2,\"place\":\"Auditorium 1\",\"speaker\":{\"name\":\"VANGELIS KAPSALAKIS\",\"title\":\"SSP Cloud Infrastructure @ Microsoft\",\"info\":\"Vangelis is a Cloud Infrastructure Solution Specialist in Microsoft Hellas covering Microsoft Enterprise Accounts in Greece, Cyprus and Malta. He started his carrier as an IT Consultant and as a Premier Field Engineer in Microsoft Hellas. He is an active member and moderator in autoexec.gr.\",\"socials\":[\"https://www.linkedin.com/in/vangelis-kapsalakis-86099438/\"]}},{\"name\":\"Lunch Break\",\"from\":\"2017-04-22 13:45\",\"to\":\"2017-04-22 14:30\",\"type\":4,\"place\":\"Auditorium 1\"},{\"name\":\"Azure SQL Data Warehouse\",\"info\":\"Azure SQL Data Warehouse is a fully-managed and scalable cloud service and with this presentation we will discover it.\",\"from\":\"2017-04-22 14:30\",\"to\":\"2017-04-22 15:30\",\"type\":2,\"place\":\"Auditorium 1\",\"speaker\":{\"name\":\"ANTONIOS CHATZIPAVLIS\",\"title\":\"SQL Server Data Platform Expert @ SiEBEN\",\"info\":\"Antonios has been involved with computers since 1982. He started his professional carrier in IT, especially in software development in 1988. In 1998 he started his carrier as Microsoft Certified Trainer in Microsoft development products. He is also the founder of sqlschool.gr and a moderator of autoexec.gr\",\"socials\":[\"http://www.sqlschool.gr/\",\"https://www.facebook.com/achatzipavlis\",\"https://twitter.com/antoniosch\",\"https://www.linkedin.com/in/achatzipavlis\"]}},{\"name\":\"Data Science with Azure Machine Learning and\u00a0R\",\"info\":\"Introduce data science with focus on Azure ML, R, Jupiter Notebook and  expose full production Analytical Predictive Services using ASP.NET Core.\",\"from\":\"2017-04-22 15:30\",\"to\":\"2017-04-22 16:30\",\"type\":2,\"place\":\"Auditorium 1\",\"speaker\":{\"name\":\"CHRISTOS CHARMATZIS\",\"title\":\"Data Scientist / GIS Specialist @ MarineTraffic\",\"info\":\"Christos Charmatzis is a Data Scientist, focusing on GIS systems, with an experience of over 6 years in the design and development of GIS projects. He holds a Microsoft certification through program Microsoft Professional Program for Data Science.\",\"socials\":[\"https://www.linkedin.com/in/christoscharmatzis/\",\"https://github.com/Charmatzis\"]}},{\"name\":\"Break\",\"from\":\"2017-04-22 16:30\",\"to\":\"2017-04-22 16:45\",\"type\":3,\"place\":\"Auditorium 1\"},{\"name\":\"Labs\",\"from\":\"2017-04-22 16:45\",\"to\":\"2017-04-22 18:00\",\"type\":5,\"place\":\"Auditorium 1\"},{\"name\":\"Closing / Gifts\",\"from\":\"2017-04-22 18:00\",\"type\":6,\"place\":\"Auditorium 1\"},{\"name\":\"Azure Active Directory: 5 reasons to implement it today!\",\"info\":\"Join me for an overview of the capabilities of Azure Active Directory, Microsoft’s Identity and Access management cloud solution (IDaaS). During this session full of demos, we'll see why it is so important to implement it today and modernize your IT infrastructure using Azure AD.\",\"from\":\"2017-04-22 09:30\",\"to\":\"2017-04-22 10:30\",\"type\":2,\"place\":\"Auditorium 2\",\"speaker\":{\"name\":\"CHRIS SPANOUGAKIS\",\"title\":\"Senior IT Consultant (independent contractor), SystemPlus IT Consulting\",\"info\":\"Senior IT Systems Engineer and Professional Trainer with extensive real-world experience and a track record of providing IT Services to senior level individuals and companies as an independent contractor.\",\"socials\":[\"https://www.linkedin.com/in/chrisspanougakis/\"]}},{\"name\":\"Web App on Linux Service, First Look\",\"info\":\"App Service on Linux enables customers to run their web apps natively on a Linux platform. Have a first inside, how Web App Service on Linux allows better application compatibility for certain kinds of applications and makes it easier to migrate existing web apps hosted on a Linux platform elsewhere onto Azure App Services.\",\"from\":\"2017-04-22 10:30\",\"to\":\"2017-04-22 12:30\",\"type\":2,\"place\":\"Auditorium 2\",\"speaker\":{\"name\":\"GEORGE CAPNIAS\",\"title\":\"Software Architect & Development Manager\",\"info\":\"Leader of dotNETZone.gr, an Athens, Greece, user group for Greek software developers using Microsoft’s .NET platform. Microsoft Certified Professional and MVP for 'Visual Studio and Development Technologies'\",\"socials\":[\"https://www.dotnetzone.gr/\",\"https://www.facebook.com/gcapnias\",\"https://twitter.com/gcapnias\",\"https://gr.linkedin.com/in/gcapnias\"]}},{\"name\":\"Break\",\"from\":\"2017-04-22 11:30\",\"to\":\"2017-04-22 11:45\",\"type\":3,\"place\":\"Auditorium 2\"},{\"name\":\"Azure IoT - IoT Hub, Device Management and IoT SDK’s\",\"info\":\"The Internet of Things connects billions of devices, sensors, data and cloud applications. In a short timeframe we will try to experience a complete IoT introduction using Arduino’s and Raspberry Pi’s, connect the devices to Azure cloud services, collect messages from sensors and send them to IoT Hub. By making use of various programming languages C#, C, Python and using open source SDK’s in multiple operating systems, the developers will get a clear understanding on how all these bind together, their capabilities and the effort required to implement and deploy IoT applications.\",\"from\":\"2017-04-22 11:45\",\"to\":\"2017-04-22 12:30\",\"type\":2,\"place\":\"Auditorium 2\",\"speaker\":{\"name\":\"VASILIS AIVALIS\",\"title\":\"Senior Software Engineer @ Skufler\",\"info\":\"Vasilis holds a BSc. and MSc. in Computer Science since 2003. He has been working as a software developer for Siemens, Atos-Origin and ZuluTrade. Throughout his career Vasilis has been given the opportunity to work with various industries, platforms and programming languages.\",\"socials\":[\"https://www.facebook.com/vasilis.aivalis\",\"https://www.linkedin.com/in/aivalisvasilis\"]}},{\"name\":\"Azure Mobile Apps and Xamarin:\u00a0From zero to hero\",\"info\":\"Mobile Apps in Azure App Service offer a highly scalable, globally available mobile application development platform that brings a rich set of capabilities to mobile developers. With Mobile Apps it’s easy to rapidly build engaging cross-platform and native apps for iOS, Android, Windows, or Mac; store app data in the cloud or on-premises with offline syncing functionality; authenticate users by selecting from an ever-growing list of identity providers; send push notifications; or add your custom backend logic in C# or Node.js. This presentation demonstrates how to get started with Azure Mobile Apps and easily build an Azure backed cross-platform mobile application.\",\"from\":\"2017-04-22 12:30\",\"to\":\"2017-04-22 13:15\",\"type\":2,\"place\":\"Auditorium 2\",\"speaker\":{\"name\":\"NASOS LOUKAS\",\"title\":\"Mobile Team Leader @ Kyon\",\"info\":\"Nasos holds an MSc in Information and Communication Systems Security and is a Software Engineer with a great passion for mobile computing, having experience in all major mobile platforms. For the last 5 years he has been working for ZuluTrade and KYON as a Mobile Software Engineer having also the role of the Mobile Team Leader.\",\"socials\":[\"https://www.facebook.com/fb.sos\",\"https://www.linkedin.com/in/aloukas\"]}},{\"name\":\"Conversations as a Platform: Bots and AI powered by Cloud\",\"info\":\"Why Bots everywhere all of the sudden? What is their purpose? Are they going to replace applications? Is Artificial Intelligence what makes them so powerful? Let’s see the answers to all of these, examine examples/best practices of Bots that are capable of mimicking human behavior and deep dive on how we can build, train and publish Bots in multiple channels using the Microsoft Bot Framework, Microsoft Azure and Cognitive Services.\",\"from\":\"2017-04-22 13:15\",\"to\":\"2017-04-22 14:00\",\"type\":2,\"place\":\"Auditorium 2\",\"speaker\":{\"name\":\"SOPHIE CHANIALAKI\",\"title\":\"Technical Evangelist @ Microsoft Hellas\",\"info\":\"Sophie holds a BSc. and MSc. in Network-Oriented Digital Systems and during her studies she was part of the Microsoft Student Partner Program for 2,5 years. After graduating, she started working as a Technical Evangelist and a MACH hire in the Developer Experience Department at Microsoft Greece, where she has developed expertise in Cloud Computing. Sophie is also the MACH Lead for Microsoft Greece, Malta and Cyprus subsidiaries and Community Organizer for Influencers such as MVPs, MSPs and other developer groups.\",\"socials\":[\"https://www.facebook.com/sofihanialaki\",\"https://twitter.com/sofiehn9\",\"https://www.linkedin.com/in/sophiehn\",\"https://github.com/sophiehn\"]}},{\"name\":\"Lunch Break\",\"from\":\"2017-04-22 13:45\",\"to\":\"2017-04-22 14:30\",\"type\":4,\"place\":\"Auditorium 2\"},{\"name\":\"Azure Application Insights\",\"info\":\"Track your app’s availability, performance and success\",\"from\":\"2017-04-22 14:30\",\"to\":\"2017-04-22 15:30\",\"type\":2,\"place\":\"Auditorium 2\",\"speaker\":{\"name\":\"DIMOSTHENIS STELLAKIS\",\"title\":\"Solutions Architect\",\"info\":\"Dimosthenis Stellakis has an MSc in Electronic & Computer Engineering, and has been involved as a Solutions Architect in many projects in Greece and abroad, using the latest Microsoft technologies. He especially likes the Web and the cloud!\",\"socials\":[\"https://www.facebook.com/dstellakis\",\"https://www.linkedin.com/in/dstellakis\"]}},{\"name\":\"Protect your data with a modern backup, archive and disaster recovery solution\",\"info\":\"Bad things happen, even to good people. Protect yourself and avoid costly business interruptions by implementing a modern backup, archive and disaster recovery strategy. See how you can securely extend your on-premises backup storage and data archive solutions to the cloud to reduce cost and complexity, while achieving efficiency and scalability. Be ready with a business continuity plan that includes disaster recovery of all your major IT systems without the expense of secondary infrastructure. You leave this session with a set of recommended architectures showing how to implement a modern backup, archive and disaster recovery solution and an understanding of how to quickly get something in place in your organization.\",\"from\":\"2017-04-22 15:30\",\"to\":\"2017-04-22 16:30\",\"type\":2,\"place\":\"Auditorium 2\",\"speaker\":{\"name\":\"PANTELIS APOSTOLIDIS\",\"title\":\"Solution Architect @ Office Line SA\",\"info\":\"A technology enthusiast IT Professional, inspired by the IT Infrastructure & the Cloud; holding Microsoft Certifications like Microsoft Certified Solutions Expert Productivity, Cloud Platform & Infrastructure. Moderator of autoexec.gr & blogger.\",\"socials\":[\"http://www.e-apostolidis.gr/\",\"https://www.facebook.com/pantelis.apostolidis\",\"https://www.linkedin.com/in/papostolidis/\",\"https://twitter.com/papostolidis\"]}},{\"name\":\"Break\",\"from\":\"2017-04-22 16:30\",\"to\":\"2017-04-22 16:45\",\"type\":3,\"place\":\"Auditorium 2\"}]}");

				var ev = AutoMapper.Mapper.Map<Event>(data);

				await eventTable.InsertAsync(ev);
				//todo replace the Constants.EventId with the ev.Id

				Debug.WriteLine($"Event: {ev}");

				foreach(var activityData in data.Activities){

					var activity = AutoMapper.Mapper.Map<Activity>(activityData);

					if(activity.Type==ActivityType.Talk){
						var speaker = AutoMapper.Mapper.Map<Speaker>(activityData.Speaker);

						await speakerTable.InsertAsync(speaker);

						activity.SpeakerId = speaker.Id;
					}

					// insert activity
					activity.EventId = ev.Id;

					await activityTable.InsertAsync(activity);
				}

				Debug.WriteLine($"Pushing...");

				// push
				await PushAsync();

				Debug.WriteLine($"Done!!");
			}
			catch (Exception e)
			{
				Debug.WriteLine($"Migrate(): {e}");
			}
		}
#endif
	}
}
