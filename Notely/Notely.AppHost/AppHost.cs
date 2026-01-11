var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sqlserver")
       .WithHostPort(5432)
       .WithDataVolume()
       .WithLifetime(ContainerLifetime.Persistent);

//var postgresSql = builder.AddPostgres("postgres")
//       .WithHostPort(5432)
//       .WithLifetime(ContainerLifetime.Persistent);

var notesDatabase = sqlServer.AddDatabase("notely-notes");
//var notesDatabase = postgresSql.AddDatabase("notely-notes");
var tagsDatabase = sqlServer.AddDatabase("notely-tags");
//var tagsDatabase = postgresSql.AddDatabase("notely-tags");

//var tagsApi = builder.AddProject<Projects.Notes_Api>("notes-api")
//        .WithReference(notesDatabase)
//        .WaitFor(notesDatabase);

//builder.AddProject<Projects.Tags_Api>("tags-api")
//    .WithHttpsEndpoint(5001, name: "public")
//    .WithReference(tagsDatabase)
//    .WithReference(tagsApi)
//    .WaitFor(tagsDatabase)
//    .WaitFor(tagsApi);

var tagsApi = builder.AddProject<Projects.Tags_Api>("tags-api")
        .WithReference(tagsDatabase)
        .WaitFor(tagsDatabase);

builder.AddProject<Projects.Notes_Api>("notes-api")
    .WithHttpsEndpoint(5001, name: "public")
    .WithReference(notesDatabase)
    .WithReference(tagsApi)
    .WaitFor(notesDatabase)
    .WaitFor(tagsApi);

builder.Build().Run();
