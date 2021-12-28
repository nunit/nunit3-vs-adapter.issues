namespace FSharpLibrary

module FSharpTypes =
            
    type DU1 =
        | DU1Member of string
    
    type DU2 =
        | DU2MemberOfDU1 of DU1
        
    type DU3 =
        | DU3Member of string

    type MyTestItem =
        {
            AofDU2: DU2
            BofDU3: DU3
        }
