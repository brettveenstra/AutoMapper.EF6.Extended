var gulp = require('gulp');
var msbuild = require('gulp-msbuild');
var assemblyInfo = require('gulp-dotnet-assembly-info');
var request = require('request');
var fs = require('fs');
var nuget = require('gulp-nuget');
var plumber = require('gulp-plumber');
var xunit = require('gulp-xunit-runner');
var runSequence = require('run-sequence');


var pkg = require('./package.json');

var config = {
  projectName: 'AutoMapper.EF6.Extended',
  buildConfig: 'Release'
}

var paths = {
  nuget: './tools/nuget/nuget.exe',
  sln: './' + config.projectName + '.sln',
  src: './src/' + config.projectName + '/',
  nugetpackage: './build/' + config.projectName + '.' + pkg.version + '.nupkg',
  tests: './src/' + config.projectName + '.Tests/',
  xunitrunner: './packages/xunit.runner.console.2.1.0/tools/xunit.console.exe'
};

gulp.task('assemblyinfo', function() {
    gulp.src(paths.src + '**/AssemblyInfo.cs')
        .pipe(assemblyInfo({
            description: pkg.description, 
            copyright: '', 
            company: pkg.author,
            version: pkg.version,
            fileVersion: pkg.version
        }))
        .pipe(gulp.dest(paths.src + '.'));
});

gulp.task('nuget-download', function(done){
      if(fs.existsSync(paths.nuget)) {
        return done();
    }

    request.get('http://nuget.org/nuget.exe')
        .pipe(fs.createWriteStream(paths.nuget))
        .on('close', done);
});

gulp.task('nuget-pack', ['nuget-download'], function() {
  var options = {
    nuget: paths.nuget,
    verbose: true,
    version: pkg.version,
    basePath: paths.src,
    properties: 'Configuration=' + config.buildConfig,
    msbuildVersion: 14,
    build: true,
    symbols: true,
    excludeEmptyDirectories: true,
    includeReferencedProjects: true,
    noDefaultExclude: true,
    tool: false
  };
  
  return gulp.src(paths.src + config.projectName + '.csproj')
    .pipe(nuget.pack(options))
    .pipe(gulp.dest('./build'));
});

gulp.task('nuget-restore', ['nuget-download'], function() {
  return gulp.src(paths.sln)
    .pipe(nuget.restore({ nuget: paths.nuget }));
});

gulp.task('nuget-push', function(){
    return gulp.src(paths.nugetpackage)
      .pipe(nuget.push({
        nuget: paths.nuget
      }));
});

gulp.task('build', function(){
    return gulp.src(paths.sln)
             .pipe(msbuild({
               stdout: true,
               stderr: true,
               verbosity: 'minimal',
               errorOnFail: true,
               targets: [ 'Clean', 'Build' ],
               configuration: config.buildConfig,
               toolsVersion: 14.0
             }));

});

gulp.task('test', function(){
  var testsPath = paths.tests + 'bin/' + config.buildConfig + '/**.Tests.dll';
 
  return gulp.src([testsPath], {read: false})
    .pipe(xunit({
      executable: paths.xunitrunner,
    }));
});

gulp.task('default', function(callback){
  runSequence(
    ['assemblyinfo', 'nuget-restore'], 
    'build',
    callback);
});

gulp.task('package', function(callback){
  runSequence(
    ['assemblyinfo', 'nuget-restore'], 
    'build',
    'test',
    'nuget-pack',
    callback);
});
