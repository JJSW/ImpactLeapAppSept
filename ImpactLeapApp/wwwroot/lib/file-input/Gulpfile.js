/**
 * Created by nicolasbui on 03/06/15.
 */
var gulp = require('gulp');
var less = require('gulp-less');
var minify = require('gulp-minify-css');
var rename = require('gulp-rename');

gulp.task('css', function () {
    var opts = {comments:true,spare:true};
    return gulp.src('./less/fileinput.less')
        .pipe(less({
            paths: [ "../bootstrap/less" ]
        }))
        .pipe(gulp.dest('./dist'))
        .pipe(minify(opts))
        .pipe(rename('fileinput.min.css'))
        .pipe(gulp.dest('./dist'));
});

gulp.task('default', ['css']);