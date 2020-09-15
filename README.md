# Azure Search over HBI documents

Need help with syntax?  Check [here!](https://github.com/mermaidjs/mermaid-gitbook/blob/master/content/sequenceDiagram.md)

## Index over temporary blob storage

```mermaid
sequenceDiagram

    participant O as Orchestrator
    participant S as HBI Source
    participant T as Transient Source
    participant TM as Transient Source Metadata
    participant AS as Azure Search

    O->>+S: Get content metadata
    Note right of O: This could be a push or pull model.
    S->>-O: Pull: full scan [name, changed-date]

    loop Update Metadata
        O->>TM: Get [name]
        alt name exists
            O->>TM: Update changed-date
        else name !exists
            O->>TM: Add [name, changed-date]
        end
    end


    O->>AS: Batch remove deleted content from index
    Note right of O: Anything in our metadata not in HBI Source Scan
    O->>TM: Batch remove deleted content from metadata

    O->>T: Copy new and changed content (with added metadata)

    O->>+AS: Initiate additive indexing
    AS->>-O: Done Indexing

    O->>T: Remove content
```

NOTE: More robust error handling needs to be added.

For Azure Search to track deletes for the index, blob storage must use 'soft delete'.

_The purpose of a data deletion detection policy is to efficiently identify deleted data items. Currently, the only supported policy is the Soft Delete policy, which allows identifying deleted items based on the value of a 'soft delete' column or property in the data source._

- [Blob soft delete](https://azure.microsoft.com/en-us/blog/soft-delete-for-azure-storage-blobs-ga/)
- [Azure search Add/Delete/Update rest api](https://docs.microsoft.com/en-us/rest/api/searchservice/addupdate-or-delete-documents)
