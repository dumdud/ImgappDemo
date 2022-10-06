// Adds an onClick event listener to the vote buttons after the DOM is loaded.
//  Then the handler function constructs the request and posts it with sendData.
document.addEventListener("DOMContentLoaded", () => {
  $(".vote-button").on("click", function () {
    let voteDir;
    let arrow = $(this).children("div");
    if (
      arrow.hasClass("icon-upvote_fill") ||
      arrow.hasClass("icon-downvote_fill")
    ) {
      voteDir = 0;
    } else if (arrow.hasClass("icon-upvote")) {
      voteDir = 1;
    } else {
      voteDir = -1;
    }

    let data = {
      Postid: $(this).attr("id"),
      Vote: voteDir,
    };

    var modal = new bootstrap.Modal(document.getElementById("loginModal"));
    sendData(data, modal, this);
  });
});

// Ajax for vote requests. Toggles the buttons on/off when successful or shows the login modal when unauthorized by the server.
function sendData(data, modal, elem) {
  $.ajax({
    type: "POST",
    url: "/api/Vote",
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    data: JSON.stringify(data),
    success: function () {
      fontbutton(elem);
    },
    error: function (response) {
      if (response.status == 401) {
        modal.show();
      }

      if (response.status == 400) {
        alert("you fucked up");
      }
    },
  });
}

function fontbutton(elem) {
  let arrow = $(elem).children("div");
  let siblingArrow = $(elem).siblings("button").children("div");

  if (arrow.hasClass("icon-upvote_fill") || arrow.hasClass("icon-upvote")) {
    arrow.toggleClass("icon-upvote icon-upvote_fill");

    if (siblingArrow.hasClass("icon-downvote_fill")) {
      siblingArrow.toggleClass("icon-downvote icon-downvote_fill");
    }
  }

  if (arrow.hasClass("icon-downvote_fill") || arrow.hasClass("icon-downvote")) {
    arrow.toggleClass("icon-downvote icon-downvote_fill");

    if (siblingArrow.hasClass("icon-upvote_fill")) {
      siblingArrow.toggleClass("icon-upvote icon-upvote_fill");
    }
  }
}
