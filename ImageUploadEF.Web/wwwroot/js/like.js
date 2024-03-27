$(() => {
    //$.get("/home/getsession", function (ids) {
    //    const imageId = $("#image-id").val()

    //    console.log({ ids })

    //    if (ids.some(id => id === imageId)) {
    //        $("#like-button").prop('disabled', true)
    //    }
    //})

    $("#like-button").on('click', function () {

        //console.log('like button clicked')

        $("#like-button").prop('disabled', true)
        //const imageId = $("#image-id").val()
        //console.log({ imageId })

        $.post(`/home/SetSession?id=${$("#image-id").val()}`), function () {
            //$.get("/home/getsession", function (ids) {
            //    if (ids.some(i => i === imageId)) {
            //        $("#like-button").prop('disabled', true)
            //    }
            //})
        }

        $.post(`/home/IncrementLikesForImage?id=${$("#image-id").val()}`), function () {
            getLikesForImage($("#image-id").val())
        }
    })

    setInterval(function () {
        //console.log('function called')

        getLikesForImage($("#image-id").val())
        
    }, 1000)

    function getLikesForImage() {
        $.get('/home/GetLikesById', { id: $("#image-id").val() }, function (likes) {
            $("#likes-count").text(likes)
        })
    }
})

