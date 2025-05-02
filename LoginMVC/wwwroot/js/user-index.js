function bindUserDetailEvents() {
	var links = document.querySelectorAll(".user-detail");
	for (var i = 0; i < links.length; i++) {
		links[i].addEventListener("click", function (e) {
			e.preventDefault();
			var userId = this.getAttribute("data-user-id");
			var popup = document.getElementById("user-popup");
			var content = document.getElementById("user-info");
			var photo = document.getElementById("user-photo");

			popup.style.top = "15%";
			popup.style.left = "50%";
			popup.style.transform = "translate(-50%, 0)";
			popup.style.display = "block";
			content.innerHTML = "불러오는 중...";
			photo.src = "/User/Photo?id=" + encodeURIComponent(userId);

			fetch("/User/Detail?id=" + encodeURIComponent(userId))
				.then(function (res) { return res.text(); })
				.then(function (html) {
					content.innerHTML = html;
				});
		});
	}
}

document.addEventListener("DOMContentLoaded", function () {
	bindUserDetailEvents();

	var moreBtn = document.getElementById("more-btn");
	if (moreBtn) {
		moreBtn.addEventListener("click", function () {
			var url = moreBtn.getAttribute("data-next");
			var search = moreBtn.getAttribute("data-search") || "";
			var sort = moreBtn.getAttribute("data-sort") || "displayName";
			var order = moreBtn.getAttribute("data-order") || "asc";

			var requestUrl = "/User/More?url=" + encodeURIComponent(url)
				+ "&search=" + encodeURIComponent(search)
				+ "&sort=" + encodeURIComponent(sort)
				+ "&order=" + encodeURIComponent(order);

			fetch(requestUrl)
				.then(function (res) {
					var next = res.headers.get("X-Next-Link");
					if (next) {
						moreBtn.setAttribute("data-next", next);
					} else {
						moreBtn.parentNode.removeChild(moreBtn);
					}
					return res.text();
				})
				.then(function (html) {
					var tbody = document.getElementById("user-body");
					tbody.insertAdjacentHTML("beforeend", html);
					var lastRow = tbody.querySelector("tr:last-child");
					if (lastRow) {
						lastRow.scrollIntoView({ behavior: "smooth" });
					}
					bindUserDetailEvents(); // 새 항목에도 이벤트 바인딩
				});
		});
	}

	var popupClose = document.getElementById("popup-close");
	if (popupClose) {
		popupClose.addEventListener("click", function () {
			document.getElementById("user-popup").style.display = "none";
		});
	}

	document.addEventListener("click", function (e) {
		var popup = document.getElementById("user-popup");
		if (!e.target.closest || (!e.target.closest("#user-popup") && !e.target.classList.contains("user-detail"))) {
			popup.style.display = "none";
		}
	});
});
