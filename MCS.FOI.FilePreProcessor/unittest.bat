
@rem to Webkit path on unit case execution location
SET HTMLtoPdfWebkitPath=C:\AOT\FOI\Source\foi-file-conversion\MCS.FOI.FilePreProcessor\MCS.FOI.FileConversion\QtBinariesWindows

@rem path to Calender files sample path on calender converion unit tests
SET SourceRootPath=C:\AOT\FOI\Source\foi-file-conversion\MCS.FOI.FilePreProcessor\MCS.FOI.CalenderToPDF.UnitTests\SharedLAN\Req1
dotnet test MCS.FOI.CalenderToPDF.UnitTests

@rem path to excel files sample path on excel conversion unit tests
SET SourceRootPath=C:\AOT\FOI\Source\foi-file-conversion\MCS.FOI.FilePreProcessor\MCS.FOI.ExcelToPDFUnitTests\SourceExcel
dotnet test MCS.FOI.ExcelToPDFUnitTests

