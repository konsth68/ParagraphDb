namespace ParagraphDb

open DapperDb
open DbModel
open Model
open TrieWordKey


    
type IParagraph =
    //abstract member FindParagraphsFromKey : string -> FullParagraph seq
    abstract member Setup : string * bool -> unit
    abstract member FindWord : string -> ResizeArray<string>
    abstract member FindParagraphs : string -> ResizeArray<FullParagraph>


type ParagraphDbService() =
    //do
    //    let db = DapperDbObj(initDb,log)
    //    initDataMorphDb db
    //    initWordKeyTrie
    //    () 
    
    interface IParagraph with
    
        member this.Setup (dbPathString :string, log :bool) =
            let initString = $"Data Source={dbPathString}"
            let db = DapperDbObj(initString,log)
            initDataMorphDb db
            initWordKeyTrie
            ()
        member this.FindWord (partialWord :string) :ResizeArray<string> =
            let tra = ResizeArray<string>()            
            if WordKeyTrie <> null && partialWord.Length > 2 then                
                let tre = WordKeyTrie.EnumerateByPrefix(partialWord)
                let trf =
                    tre
                    |> Seq.map (fun a -> a.Key)                           
                    |> ResizeArray<string>
                trf
            else
                tra
        
        member this.FindParagraphs (partWord :string) :ResizeArray<FullParagraph> =
            let tre = WordKeyTrie.EnumerateByPrefix partWord
            let idArr =
                tre
                |> Seq.map (fun a -> a.Value.WordId)
                |> Seq.toArray
        
            let dbPar = FindWordIds idArr    
                                                   
            let res =
                dbPar
                |> Seq.groupBy (fun a -> a.WordKeyId)

            let fs  =
                res
                |> Seq.map (fun a -> CreateFullParagraph (snd a) )                                                                                                                                    
                //|> Seq.toArray
                
            let ra = ResizeArray<FullParagraph> fs
            ra

    //new () =
        //let curDir = System.IO.Directory.GetCurrentDirectory()
    //    let initDb = $"Data Source=c:\\users\\konsth\\appdata\\local\\FranDict\\FranDict.db"
        
    //    ParagraphDbService(initDb,false)