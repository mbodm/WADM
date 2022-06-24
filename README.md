# WADM
An addon download manager for World of Warcraft

#### ⛔ WADM, at the moment, is not working any longer! ⛔

WADM is an easy-to-use addon download manager for the popular video game World of Warcraft. For more information see WADM´s home at CodePlex: http://wadm.codeplex.com. I just use GitHub for more comfortable source code version control.

⚠️

#### Why WADM actually stopped working (as of June 2022)?
This has nothing to do with WADM, but Curse (now Overwolf, since a few years) finally implemented some access control in their REST API, by using API keys, to control who is using their API. So their API is now protected from being used by tools that are __not__ the official Curse download client. They also use some Cloudflare anti-scraper security mechanisms, to protect their website. So, scraping the website is also no longer an option, if not using rather complex stuff like "pupeteer" or "selenium" and other scraper tools. For more information about this topic and all the „Overwolf problems“, please have a look at the communities of the [https://github.com/ajour/ajour](Ajour) and WowUp addon management tools, or use your Google-Fu. These topics are heavily discussed there, since a good while. Overwolf always mentioned, since years, they will protect their API (to control who is using the API) and prevent their addon download site with Cloudflare. Since they made that a reality, there is not much we can do at the moment.

⚠️

#### Screenshot:
![alt text](https://github.com/MBODM/WADM/blob/master/SCREENSHOT.png)
