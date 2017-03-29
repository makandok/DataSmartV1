if object_id('DataDictionaryForTable') is not null drop function DataDictionaryForTable
go
create FUNCTION [dbo].[DataDictionaryForTable](
@project varchar(10),
@tablename varchar(255)
) RETURNS TABLE AS return 
( 
select * From DataDictionary where project = '' or (project =@project and tablename=@tablename)
) 
GO

if object_id('pp_client_eval_func') is not null drop function pp_client_eval_func
go
create FUNCTION [dbo].[pp_client_eval_func]() RETURNS TABLE AS return (
select RecordId, Serial, GeneralLabel,FullVariableName,IsCodedValue,Value,GeneralVariableName
from (
	select RecordId, Serial, 
	 GeneralLabel
	, case when len(groupkey) > 0 then groupkey +'_'+ VariableName else VariableName end FullVariableName
	, case when lookupcode is null then 0 else 1 end as IsCodedValue
			, case when datatype = 'checkbox' then '1'
	else case when lookupcode is null then fieldvalue else convert(varchar(50),lookupcode) end 
	end as Value,
	VariableName as GeneralVariableName,
	label DescriptiveLabel, longname uniquename
	From 
	(
		select FieldValue, case when RecordId is null then 0 else RecordId end as RecordId, d.* 
		from DataDictionaryForTable('ppx','pp_client_eval') d 
		left join 
		(
		select * from 
		pp_client_eval_local_fvs where recordid is not null
		) fvs on d.longname = fvs.fieldname

	)b
) dataset
)
GO

if object_id('pp_client_devicerem_func') is not null drop function pp_client_devicerem_func
go
create FUNCTION [dbo].[pp_client_devicerem_func]() RETURNS TABLE AS return (
select RecordId, Serial, GeneralLabel,FullVariableName,IsCodedValue,Value,GeneralVariableName
from (
	select RecordId, Serial, 
	 GeneralLabel
	, case when len(groupkey) > 0 then groupkey +'_'+ VariableName else VariableName end FullVariableName
	, case when lookupcode is null then 0 else 1 end as IsCodedValue
	--, case when lookupcode is null then fieldvalue else convert(varchar(50),lookupcode) end as Value,
		, case when datatype = 'checkbox' then '1'
	else case when lookupcode is null then fieldvalue else convert(varchar(50),lookupcode) end 
	end as Value,
	VariableName as GeneralVariableName,
	label DescriptiveLabel, longname uniquename
	From 
	(
		select FieldValue, case when RecordId is null then 0 else RecordId end as RecordId, d.* 
		from DataDictionaryForTable('ppx','pp_client_devicerem') d 
		left join 
		(
		select * from 
		pp_client_devicerem_local_fvs where recordid is not null
		) fvs on d.longname = fvs.fieldname
	)b
) dataset
)
GO

if object_id('pp_client_postrem_func') is not null drop function pp_client_postrem_func
go
ALTER FUNCTION [dbo].[pp_client_postrem_func]() RETURNS TABLE AS return (
select RecordId, Serial, GeneralLabel,FullVariableName,IsCodedValue,Value,GeneralVariableName
from (
	select RecordId, Serial, 
	 GeneralLabel
	, case when len(groupkey) > 0 then groupkey +'_'+ VariableName else VariableName end FullVariableName
	, case when lookupcode is null then 0 else 1 end as IsCodedValue
				, case when datatype = 'checkbox' then '1'
	else case when lookupcode is null then fieldvalue else convert(varchar(50),lookupcode) end 
	end as Value,
	VariableName as GeneralVariableName,
	label DescriptiveLabel, longname uniquename
	From 
	(
		select FieldValue, case when RecordId is null then 0 else RecordId end as RecordId, d.* 
		from DataDictionaryForTable('ppx','pp_client_postrem') d 
		left join 
		(
		select * from 
		pp_client_postrem_local_fvs where recordid is not null
		) fvs on d.longname = fvs.fieldname
	)b
) dataset
)
GO

if object_id('pp_client_unsched_func') is not null drop function pp_client_unsched_func
go
create FUNCTION [dbo].[pp_client_unsched_func]() RETURNS TABLE AS return (
select RecordId, Serial, GeneralLabel,FullVariableName,IsCodedValue,Value,GeneralVariableName
from (
	select RecordId, Serial, 
	 GeneralLabel
	, case when len(groupkey) > 0 then groupkey +'_'+ VariableName else VariableName end FullVariableName
	, case when lookupcode is null then 0 else 1 end as IsCodedValue
					, case when datatype = 'checkbox' then '1'
	else case when lookupcode is null then fieldvalue else convert(varchar(50),lookupcode) end 
	end as Value,
	VariableName as GeneralVariableName,
	label DescriptiveLabel, longname uniquename
	From 
	(
		--select FieldValue, RecordId, d.* from
		-- pp_client_unsched_local_fvs fvs 
		-- left join DataDictionary d on fvs.fieldname = d.longname
		--where project = '' or (project ='ppx' and tablename='pp_client_unsched')

		select FieldValue, case when RecordId is null then 0 else RecordId end as RecordId, d.* 
		from DataDictionaryForTable('ppx','pp_client_unsched') d 
		left join 
		(
		select * from 
		pp_client_unsched_local_fvs where recordid is not null
		) fvs on d.longname = fvs.fieldname

	)b
) dataset
)
GO

if object_id('vmmc_regandproc_func') is not null drop function vmmc_regandproc_func
go
CREATE FUNCTION [dbo].[vmmc_regandproc_func]() RETURNS TABLE AS return (
select RecordId, Serial, GeneralLabel,FullVariableName,IsCodedValue,Value,GeneralVariableName
from (
	select RecordId, Serial, 
	 GeneralLabel
	, case when len(groupkey) > 0 then groupkey +'_'+ VariableName else VariableName end FullVariableName
	, case when lookupcode is null then 0 else 1 end as IsCodedValue
	, case when datatype = 'checkbox' then '1'
	else case when lookupcode is null then fieldvalue else convert(varchar(50),lookupcode) end 
	end as Value,
	VariableName as GeneralVariableName,
	label DescriptiveLabel, longname uniquename
	From 
	(
		select FieldValue, case when RecordId is null then 0 else RecordId end as RecordId, d.* 
		from DataDictionaryForTable('vmc','vmmc_regandproc') d 
												left join 
		(
		select * from 
		vmmc_regandproc_local_fvs where recordid is not null
		) fvs on d.longname = fvs.fieldname
	)b
) dataset
)
GO

if object_id('vmmc_postop_func') is not null drop function vmmc_postop_func
go
CREATE FUNCTION [dbo].[vmmc_postop_func]() RETURNS TABLE AS return (
select RecordId, Serial, GeneralLabel,FullVariableName,IsCodedValue,Value,GeneralVariableName
from (
	select RecordId, Serial, 
	 GeneralLabel
	, case when len(groupkey) > 0 then groupkey +'_'+ VariableName else VariableName end FullVariableName
	, case when lookupcode is null then 0 else 1 end as IsCodedValue
	, case when datatype = 'checkbox' then '1'
	else case when lookupcode is null then fieldvalue else convert(varchar(50),lookupcode) end 
	end as Value,
	VariableName as GeneralVariableName,
	label DescriptiveLabel, longname uniquename
	From 
	(
		select FieldValue, case when RecordId is null then 0 else RecordId end as RecordId, d.* 
		from DataDictionaryForTable('vmc','vmmc_postop') d 
		left join 
		(
		select * from 
		vmmc_postop_local_fvs where recordid is not null
		) fvs on d.longname = fvs.fieldname
	)b
) dataset
)
GO



if object_id('allData') is not null drop view allData
go
create view allData as
--  select * from allData
select 'pp_client_eval' tablename, * from pp_client_eval_func() union
select 'pp_client_devicerem' tablename,* from pp_client_devicerem_func() union
select 'pp_client_postrem' tablename,* from pp_client_postrem_func() union
select 'pp_client_unsched' tablename,* from pp_client_unsched_func() union

select 'vmmc_regandproc' tablename,* from vmmc_regandproc_func() union
select 'vmmc_postop' tablename,* from vmmc_postop_func()
go

