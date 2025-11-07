namespace ParagraphDb

open DapperDb

type PosTagDb =      
    |None              =  0  
    |Noun              =  1  
    |Adjective         =  2  
    |Verb              =  3  
    |Adverb            =  4  
    |Numeral           =  5  
    |Participle        =  6  
    |Transgressive     =  7  
    |Pronoun           =  8  
    |Preposition       =  9  
    |Conjunction       =  10 
    |Particle          =  11 
    |Interjection      =  12 
    |Predicative       =  13 
    |Parenthesis       =  14 



[<CLIMutable>]
type WorkParagraph =
    {
        WorkParagraphId :int64
        WordKeyId   :int64 
        Word        :string
        PosTag      :PosTagDb
        Index       :int
        ParagraphStr:  string
        HagenId :int64
    }

[<CLIMutable>]
type WordKey =
    {
        WordId  :int64
        Word :string
    }
        
module DbModel =

    let mutable Db = null
    
    let initDataMorphDb (db :DapperDbObj) =
        Db <- db    
        ()
    
    //Paragraph-----------------------------------------------------------------------------------------------------
    let allWorkParagraphColumns = "WorkParagraphId,WordKeyId,Word,PosTag,IndexPar,ParagraphStr,HagenId"
    let parGetAllStrSql =
        $"SELECT {allWorkParagraphColumns} FROM WorkParagraph"
    
    let parGetIdStrSql (id :int64) :string =
        $"SELECT {allWorkParagraphColumns} FROM WorkParagraph WHERE WorkParagraphId = {id} "
    
    let parGetFilterStrSql (filter :string) :string =
        $"SELECT {allWorkParagraphColumns} FROM WorkParagraph WHERE  {filter} "

    let parFindWordIdsStrSql (ids :int64 array) =  
        let inStr = System.String.Join(",",ids)
        $"SELECT {allWorkParagraphColumns} FROM WorkParagraph WHERE WordKeyId IN ( {inStr} )";

    let parFindWordIdStrSql (id :int64) =  
        $"SELECT {allWorkParagraphColumns} FROM WorkParagraph WHERE WordKeyId = {id} ";
    
    let getAllPar () =
        let sql = parGetAllStrSql
        Db.QueryManyDapper<WorkParagraph> sql
    
    let getIdPar (id :int64) =
        let sql = parGetIdStrSql id
        Db.QueryOneDapper<WorkParagraph> sql
    
    let getFilterPar (filter :string) =
        let sql = parGetFilterStrSql filter
        Db.QueryManyDapper<WorkParagraph> sql 
    
    let FindWordIds (ids :int64 array) =
        let sql = parFindWordIdsStrSql ids
        let r = Db.QueryManyDapper<WorkParagraph> sql
        checkSeqOption r
    
    let FindWordId (id :int64) =
        let sql = parFindWordIdStrSql id
        Db.QueryManyDapper<WorkParagraph> sql

    //WordKey-----------------------------------------------------------------------------------------------------        
    let wordGetAllStrSql =
        "SELECT WordId,Word FROM WordKey"
    
    let wordGetIdStrSql (id :int64) :string =
        $"SELECT WordId,Word FROM WordKey WHERE WordId = {id} "
    
    let wordGetFilterStrSql (filter :string) :string =
        $"SELECT WordId,Word FROM WordKey WHERE  {filter} "

    let wordGetGetWord (word :string) :string =
        $"SELECT WordId,Word FROM WordKey WHERE Word like \'%% {word} %%\'"
                
    let getAllWord () =
        let sql = wordGetAllStrSql
        Db.QueryManyDapper<WordKey> sql
    
    let getIdWord (id :int64) =
        let sql = wordGetIdStrSql id
        Db.QueryOneDapper<WordKey> sql
    
    let getFilterWord (filter :string) =
        let sql = wordGetFilterStrSql filter
        let r = Db.QueryManyDapper<WordKey> sql 
        checkSeqOption r
    let getWord(word :string) =
        let sql = wordGetGetWord word
        let r = Db.QueryManyDapper<WordKey> sql
        checkSeqOption r