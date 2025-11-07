namespace ParagraphDb

open System
open Dapper
open Microsoft.Data.Sqlite

module Util =

    let unixStart = DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)

    let convertOptIntToDateTime (inVal: int64 option) =
        let res =
            match inVal with
            | Some x -> DateTime(unixStart.Ticks + int64 x, DateTimeKind.Utc)
            | None -> DateTime.MinValue

        res

    let convertOptStringToString (inVal: string option) =
        let res =
            match inVal with
            | Some x -> x
            | None -> ""

        res

    let convertOptIntToInt (inVal: int64 option) =
        let res =
            match inVal with
            | Some x -> x
            | None -> 0

        res

    let convertOptFloatToFloat (inVal: float option) =
        let res =
            match inVal with
            | Some x -> x
            | None -> 0.0

        res


module DapperDb =

    let inline notNull value = not (obj.ReferenceEquals(value, null))
    let inline isNull value = obj.ReferenceEquals(value, null)

    let inline checkSeqOption (value: 'T seq option) =
        match value with
        | Some v -> v
        | None -> Seq.empty


    type OptionHandler<'T>() =
        inherit SqlMapper.TypeHandler<option<'T>>()

        override _.SetValue(param, value) =
            let valueOrNull =
                match value with
                | Some x -> box x
                | None -> null

            param.Value <- valueOrNull

        override _.Parse value =
            if isNull value || value = box DBNull.Value then
                None
            else
                Some(value :?> 'T)

    type DapperDbObj(connStr: string, logFlag: bool) =
        do
            SqlMapper.AddTypeHandler(typeof<option<int>>, OptionHandler<int>())
            SqlMapper.AddTypeHandler(typeof<option<string>>, OptionHandler<string>())
            SqlMapper.AddTypeHandler(typeof<option<DateTime>>, OptionHandler<DateTime>())
            SqlMapper.AddTypeHandler(typeof<option<float>>, OptionHandler<float>())


        let LogDapperDb str =
            if logFlag then
                ParLog.Logger.Information(str)


        member this.QueryOneDapper<'T>(sql: string) : 'T option =

            use conn = new SqliteConnection(connStr)

            let res = conn.QuerySingle<'T>(sql)

            let r =
                if notNull res then
                    LogDapperDb($"Query with sql = \"{sql}\" success on {res} result ")
                    Some res
                else
                    LogDapperDb($"Query with sql = \"{sql}\" None ")
                    None

            r

        member this.QueryManyDapper<'T>(sql: string) : 'T seq option =

            use conn = new SqliteConnection(connStr)

            let res = conn.Query<'T>(sql)

            let r =
                if notNull res then
                    LogDapperDb($"Query with sql = \"{sql}\" success on count {Seq.length res} result ")
                    Some res
                else
                    LogDapperDb($"Query with sql = \"{sql}\" None ")
                    None

            r

        member this.ExecuteDapper(sql: string) : int =

            use conn = new SqliteConnection(connStr)

            let res = conn.Execute(sql)

            LogDapperDb($"Query with sql = \"{sql}\" success on result = {res} ")

            res


        member this.ExecuteTransactDapper(sql: string) : int option =

            use conn = new SqliteConnection(connStr)

            use tran = conn.BeginTransaction(System.Data.IsolationLevel.ReadCommitted)

            let r =
                try
                    let res = conn.Execute(sql, tran)
                    tran.Commit()
                    LogDapperDb($"Query with sql = \"{sql}\" success on result = {res} ")
                    Some res
                with ex ->
                    LogDapperDb($"Query with sql = \"{sql}\" Rollback and exception {ex.Message} ")
                    tran.Rollback()
                    None

            r

        member this.ExecuteOutsideTransactDapper (sql: string) (tran: SqliteTransaction) : int option =

            use conn = new SqliteConnection(connStr)

            let r =
                try
                    let res = conn.Execute(sql, tran)
                    //tran.Commit()
                    LogDapperDb($"Query with sql = \"{sql}\" success on result = {res} ")
                    Some res
                with ex ->
                    LogDapperDb($"Query with sql = \"{sql}\" Rollback and exception {ex.Message} ")
                    tran.Rollback()
                    None

            r

        member this.ExecuteTransactManyDapper(sqlList: string list) =

            use conn = new SqliteConnection(connStr)

            conn.Open()

            use tran = conn.BeginTransaction(System.Data.IsolationLevel.ReadCommitted)

            let r =
                try
                    let res = sqlList |> List.map (fun x -> conn.Execute(x, tran))
                    LogDapperDb($"Query with sql = \"{sqlList}\" success on result = {res} ")
                    tran.Commit()
                    Some res
                with ex ->
                    LogDapperDb($"Query with sql = \"{sqlList}\" Rollback and exception {ex.Message} ")
                    tran.Rollback()
                    None

            r
