<Query Kind="Statements">
  <Connection>
    <ID>80c38377-c04c-4cf6-b3b6-f2c5395c4508</ID>
    <Persist>true</Persist>
    <Server>.</Server>
    <Database>sandbox</Database>
    <NoPluralization>true</NoPluralization>
    <NoCapitalization>true</NoCapitalization>
  </Connection>
  <Reference>&lt;ProgramFilesX64&gt;\LINQPad\dll\microsoft.crm.sdk.proxy.dll</Reference>
  <Reference>&lt;ProgramFilesX64&gt;\LINQPad\dll\microsoft.xrm.sdk.dll</Reference>
  <Reference>&lt;ProgramFilesX64&gt;\LINQPad\dll\MscrmApiShell.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Data.DataSetExtensions.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Data.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Runtime.Serialization.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.ServiceModel.dll</Reference>
</Query>

/*
    Limits

    Due to the very high volume of queries we've implemented a throttling mechanism, which allows at most 1,000 queries per hour. 
    After reaching this hourly quota, all your requests result in HTTP 403 (Forbidden) until it clears up on the next roll over.

    so, delay must be about 5 seconds
	
	--- checking results
	select * from [linqpad].[binlist_web_api_responses] (NOLOCK) order by id desc

*/

var bin_list =
    (from f in bins_list
    select f.bin).ToArray().Take(1);

foreach (string bin in bin_list)
{
	try
	{
		string sURL = "https://binlist.net/json/" + bin;
	
		using(var objStream = System.Net.WebRequest.Create(sURL).GetResponse().GetResponseStream())
		{
			var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(binlist_web_api_responses)); 
			var binlist_web_api_response = (binlist_web_api_responses)serializer.ReadObject(objStream);
	
			binlist_web_api_responses.InsertOnSubmit(binlist_web_api_response);
			SubmitChanges();
		}
	
		Thread.Sleep(5000);
	}
	catch
	{
		Thread.Sleep(60000);
	}
}

/*
------ SQL Server Part
----------------------

create schema linqpad;
go

-- this supposed to be a view returning all the bins you want to get information about
create view [linqpad].[bins_list] as
select '431940' as [bin];
go

create table [linqpad].[binlist_web_api_responses](
    [id] [int] primary key identity(1,1) NOT NULL,
    [bin] [nvarchar](100) NULL,
    [brand] [nvarchar](100) NULL,
    [sub_brand] [nvarchar](100) NULL,
    [country_code] [nvarchar](100) NULL,
    [country_name] [nvarchar](100) NULL,
    [bank] [nvarchar](200) NULL,
    [card_type] [nvarchar](100) NULL,
    [card_category] [nvarchar](100) NULL,
    [latitude] [nvarchar](100) NULL,
    [longitude] [nvarchar](100) NULL
);
go

*/