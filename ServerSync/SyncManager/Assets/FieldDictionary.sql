CREATE TABLE [dbo].[UniqueIdMap](
--  drop table UniqueIdMap
	[IntegerId] [int] IDENTITY(1,1) NOT NULL,
	[StringId] [nvarchar](32) primary key
)
go

CREATE TABLE [dbo].[FieldDictionary](
--  drop table [FieldDictionary]
	[project] [varchar](255) NULL,
	[dataType] [varchar](255) NULL,
	[fieldName] [varchar](255) NULL,
	[fieldType] [varchar](255) NULL,
	[IsIndexed] [varchar](255) NULL,
	[IsRequired] [varchar](255) NULL,
	[Label] [varchar](255) NULL,
	[name] [varchar](255) NULL,
	[PageId] [varchar](255) NULL,
	[pageName] [varchar](255) NULL,
	[choiceName] [varchar](255) NULL,
	[groupKey] [varchar](255) NULL,
	[listName] [varchar](255) NULL,
	[lookupValue] [varchar](255) NULL,
	[tableName] [varchar](255) NULL,
	[Serial] [int] IDENTITY(1,1) NOT NULL
)
go

CREATE TABLE [dbo].[LookupValues](
	[LookupName] [nvarchar](255) NULL,
	[LookupValue] [nvarchar](255) NULL,
	[LookupCode] [float] NULL
)
go

alter view [dbo].[fd] as 
--  select * from [fd]
SELECT [Serial],
		[project]
      ,[dataType]
      ,[fieldName]
      ,[fieldType]
      ,[IsIndexed]
      ,[IsRequired]
      ,[Label]
      ,[name]
      ,[PageId]
      ,[pageName]
      ,[choiceName]
      ,[groupKey]
      ,[listName]
      ,[lookupValue]
      ,tableName
  FROM [dbo].[FieldDictionary]
  union
SELECT 0, '' [project]
      ,'EditText' [dataType]
      ,'id' [fieldName]
      ,'Text' [fieldType]
      ,'True' [IsIndexed]
      ,'True'[IsRequired]
      ,'id' [Label]
      ,'id' [name]
      ,'0'[PageId]
      ,'ilsp_main1'[pageName]
      ,''[choiceName]
      ,''[groupKey],'','','' tableName union
SELECT 0,'' [project]
      ,'EditText' [dataType]
      ,'entityid' [fieldName]
      ,'Text' [fieldType]
      ,'True' [IsIndexed]
      ,'True'[IsRequired]
      ,'entityid' [Label]
      ,'entityid' [name]
      ,'0'[PageId]
      ,'ilsp_main1'[pageName]
      ,''[choiceName]
      ,''[groupKey],'','','' tableName  union
	SELECT 0,'' [project]
      ,'DatePicker' [dataType]
      ,'sys_editdate' [fieldName]
      ,'Date' [fieldType]
      ,'True' [IsIndexed]
      ,'True'[IsRequired]
      ,'sys_editdate' [Label]
      ,'sys_editdate' [name]
      ,'0'[PageId]
      ,'ilsp_main1'[pageName]
      ,''[choiceName]
      ,''[groupKey],'','','' tableName  union
SELECT 0,'' [project]
      ,'DatePicker' [dataType]
      ,'sys_datecreated' [fieldName]
      ,'Date' [fieldType]
      ,'True' [IsIndexed]
      ,'True'[IsRequired]
      ,'sys_datecreated' [Label]
      ,'sys_datecreated' [name]
      ,'0'[PageId]
      ,'ilsp_main1'[pageName]
      ,''[choiceName]
      ,''[groupKey],'','','' tableName ;
GO


ALTER view [dbo].[DataDictionary] as
--  select * from DataDictionary
select serial,project,
datatype, fieldtype, label,
case when len(LookupValue) > 0 then 
	replace(label, '['+LookupValue+']','')
	else Label
	end GeneralLabel
,groupkey, name longname,
FieldNameActual VariableName,
--REPLACE(FieldNameActual,'ilsp_','') VariableName,
--FieldNameActual fieldname, 
listname, lookupvalue, choicename lookupvalueclean,
lookupcode, pagename,tablename
from (
	select f.*, lookupCode,
	--case when len(fieldname) = 0 then name else fieldname end FieldNameActual
		case when datatype='CheckBox' then name
	else case when len(fieldname) = 0 then name else fieldname end 
	end FieldNameActual
	 From fd f 
	left join LookupValues v on f.listName = v.lookupname
	and f.lookupValue = v.lookupValue
	--where (f.listname is not null and len(f.listname) > 0)
	--and v.lookupname is null
)a
go

--create view ppxFinalDataset as
--select RecordId, Serial, GeneralLabel,FullVariableName,IsCodedValue,Value,GeneralVariableName
--from (
--	select RecordId, Serial, 
--	 GeneralLabel
--	, case when len(groupkey) > 0 then groupkey +'_'+ VariableName else VariableName end FullVariableName
--	, case when lookupcode is null then 0 else 1 end as IsCodedValue
--	, case when lookupcode is null then fieldvalue else convert(varchar(50),lookupcode) end as Value,
--	VariableName as GeneralVariableName,
--	label DescriptiveLabel, longname uniquename
--	From 
--	(
--		select FieldValue, RecordId
--		, d.* from 
--		pp_client_eval_local_fvs fvs left join DataDictionary d on fvs.fieldname = d.longname
--	)b
--) dataset
----order by 1,2
--go