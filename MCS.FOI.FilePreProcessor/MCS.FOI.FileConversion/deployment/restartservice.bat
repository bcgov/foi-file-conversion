
NET STOP "MCS.FOI.FileConversionService" 

timeout /t 15 /nobreak > NUL

NET START "MCS.FOI.FileConversionService"