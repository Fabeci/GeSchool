(function () {
    "use strict";

    // ---- Sidebar mobile drawer ----
    var sidebar = document.getElementById("gsSidebar");
    var toggle = document.getElementById("gsSidebarToggle");
    var backdrop = document.getElementById("gsSidebarBackdrop");

    function closeSidebar() {
        if (sidebar) sidebar.classList.remove("show");
        if (backdrop) backdrop.classList.remove("show");
    }

    function openSidebar() {
        if (sidebar) sidebar.classList.add("show");
        if (backdrop) backdrop.classList.add("show");
    }

    if (toggle) {
        toggle.addEventListener("click", function () {
            if (sidebar && sidebar.classList.contains("show")) {
                closeSidebar();
            } else {
                openSidebar();
            }
        });
    }
    if (backdrop) {
        backdrop.addEventListener("click", closeSidebar);
    }

    // ---- Theme toggle (Light / Dark / System) ----
    var THEME_KEY = "gs-theme";

    function applyTheme(theme) {
        if (theme === "light" || theme === "dark") {
            document.documentElement.setAttribute("data-theme", theme);
            localStorage.setItem(THEME_KEY, theme);
        } else {
            document.documentElement.removeAttribute("data-theme");
            localStorage.removeItem(THEME_KEY);
        }
    }

    document.querySelectorAll("[data-theme-choice]").forEach(function (btn) {
        btn.addEventListener("click", function () {
            applyTheme(btn.getAttribute("data-theme-choice"));
        });
    });

    // ---- Generic table search filter ----
    // Usage: <input data-table-search data-table-target="#myTable">
    document.querySelectorAll("[data-table-search]").forEach(function (input) {
        var targetSelector = input.getAttribute("data-table-target");
        var table = targetSelector ? document.querySelector(targetSelector) : null;
        if (!table) return;

        input.addEventListener("input", function () {
            var term = input.value.trim().toLowerCase();
            var rows = table.querySelectorAll("tbody tr[data-row]");
            var visibleCount = 0;

            rows.forEach(function (row) {
                var text = row.textContent.toLowerCase();
                var matches = term === "" || text.indexOf(term) !== -1;
                row.style.display = matches ? "" : "none";
                if (matches) visibleCount++;
            });

            var emptyRow = table.querySelector("[data-search-empty]");
            if (emptyRow) {
                emptyRow.style.display = visibleCount === 0 ? "" : "none";
            }
        });
    });

    // ---- Confirm delete modal population ----
    document.querySelectorAll(".modal[id]").forEach(function (modal) {
        modal.addEventListener("show.bs.modal", function (event) {
            var trigger = event.relatedTarget;
            if (!trigger) return;

            var id = trigger.getAttribute("data-id");
            var label = trigger.getAttribute("data-label");

            var idInput = modal.querySelector("[data-modal-id-input]");
            var labelEl = modal.querySelector("[data-modal-label]");

            if (idInput && id !== null) idInput.value = id;
            if (labelEl && label !== null) labelEl.textContent = label;
        });
    });
})();
