namespace ParagraphDb

open DbModel
open KTrie

module TrieWordKey =
    
     let mutable WordKeyTrie = null   //= TrieDictionary<LemmaTrieData>() 
         
     let fillWords =
        let wrs = getAllWord ()        
        let wordArr = match wrs with
                        | Some x -> Seq.toArray x
                        | None -> [||]
        
        wordArr
        
     let fillTrie =
    
        let Words = fillWords  
        let trieDict = TrieDictionary<WordKey> ()
        
        if (Words.Length = 0) then
            trieDict
        else
            for w in Words do
                if(w.Word <> null) then
                    trieDict.Add(w.Word,w)
            trieDict        

     let initWordKeyTrie  =
         WordKeyTrie <- fillTrie 
         ()
