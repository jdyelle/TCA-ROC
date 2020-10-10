# Monroe County Census Data:

## Concept:
Create an Analytics System to merge various sets of data and statistics to drive insights for need-based aid.

### Data:

 - Basic Demographics
 - Family and Relationship Data
 - Income / Support Statistics
 - Disability Information
 - Housing Statistics

## Conventions:
A few conventions to get us started on logging, implementation details:

### Getting Started:
- Contact me with your IP Address so that I can whitelist/share database info for you.
- Clone the project, this project is eligible for VS2019 Community Edition.
- The IngestBase class has been provided to create a frame of what you'll need to do implementations.
- Please place individual implementation classes in the [ODL.Common\IngestImplementations] folder.
- Link the implementation to the relevant drop down in the UIMainScreen class under the [btnLoadSelectedFile_Click] method (there should be a logic tree there for the dropdown).
- Take a look at issues #24, 25, 26, and 27. Those have a limited scope. (Beginner-friendly issues should be tagged with "good first issue")

### Logging:
 - LogTrace(String) : Method Exit/Entry Logging : Log only if Logging.DebugMode is set
 - LogDebug(String) : Informational Parts of Code, Surfacing a paramter or method stack : Logging.DebugMode
 - LogInformation(String) : General User-facing informational messages
 - LogWarning(String) : Warning messages that a user should be aware of (may not be errors)
 - LogError(String) : Errors in expected behavior of the software (file can't save, database can't connect, etc)

