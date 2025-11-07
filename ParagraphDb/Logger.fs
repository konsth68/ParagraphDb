namespace ParagraphDb

open Serilog

[<RequireQualifiedAccess>]
module ParLog =

    let loggerConfig =
        LoggerConfiguration().MinimumLevel.Debug().WriteTo.File(".\ParagraphService.log").CreateLogger()


    let Logger = loggerConfig
