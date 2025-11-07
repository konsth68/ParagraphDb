namespace ParagraphDb


type FullParagraph =
    {
        WordId :int64 
        Word   :string

        Paragraphs : ResizeArray<WorkParagraph> 
    }

module Model =
        
    let CreateFullParagraph (parSq :WorkParagraph seq) =                
        let pa = parSq |> Seq.toArray

        let fp :FullParagraph =
            {
                WordId = pa[0].WordKeyId
                Word = pa[0].Word
                Paragraphs = ResizeArray<WorkParagraph> pa      
            }                                 
        fp 