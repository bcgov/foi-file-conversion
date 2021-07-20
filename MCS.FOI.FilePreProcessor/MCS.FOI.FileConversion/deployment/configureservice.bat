
@rem to Webkit path on unit case execution location

sc create MCS.FOI.FileConversionService BinPath=C:\AOT\FOI\FileConversionPub\MCS.FOI.FileConversion.exe start=auto

sc start MCS.FOI.FileConversionService 

sc description MCS.FOI.FileConversionService "This service is owned by FOI Technical team, used for File Conversion Process."