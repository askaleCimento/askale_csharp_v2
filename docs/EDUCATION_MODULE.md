# Education module

The module follows the existing solution layers:

- AskalePortal/Controllers/Education: HTTP contract only
- AskalePortal.BLL/Education: Java service behavior and file rules
- AskalePortal.DAL/Education: EF Core repositories and Java JPQL equivalents
- AskalePortal.Data/Models: database-first entities
- AskalePortal.Data/RequestModels/Education: filter requests
- AskalePortal.Data/ResponseModels/Education: nested Flutter response DTOs

## Java-compatible endpoints

Every controller includes the inherited endpoints:

- POST save
- POST delete (form-data: id)
- POST getById (form-data: id)
- POST getAll
- POST filterPageable
- POST getAllFilter

Special endpoints:

- POST api/education/filterByPageable
- POST api/education/listByEgitimBolumId
- POST api/educationsection/filterPageable
- POST api/educationvideo/filterByPageable
- POST api/educationvideo/upload
- POST api/educationvideo/download
- GET api/educationvideo/getVideo/{fileName}
- POST api/educationvideoduration/listByVideoId
- POST api/educationvideoduration/listByVideoIdAndUserId
- POST api/egitimsorulari/listByVideoId
- POST api/egitimsorucevap/listByVideoId
- POST api/egitimsorucevap/listByVideoIdAndUserId
- POST api/educationquestion/listBySectionId
- POST api/educationquestionsection/filterByPageable
- POST api/educationquestionanswer/listBySectionId
- POST api/educationquestionanswer/listBySectionIdAndUserId

## Flutter compatibility endpoints

The supplied Flutter project also calls these endpoints, which are included:

- POST api/educationvideo/uploadVideo
- POST api/educationvideo/uploadImage
- GET api/educationvideo/downloadPicture/{videoId}

## Behavior migrated from Java

- Audit fields on insert/update
- Soft delete by enabled=false
- Role 1 sees all education/question-section rows; other users see their own rows
- Education responses contain videos; videos contain questions
- Question-section responses contain questions
- Previous active viewing duration is disabled before a new one is inserted
- Previous active answer is disabled before a new legacy answer is inserted
- New question answer updates the existing active answer for the same user/question
- Video and image uploads update the entity and create AttachedFile records with module 56
- Secure file-name normalization and range-enabled video streaming
- Spring Page-compatible response shape

## Configuration

The active database/file profile is selected with ASKALE_ENVIRONMENT: local, test, or server.
The module reads:

- Connectionstrings:{profile}
- FilePath:{profile}
