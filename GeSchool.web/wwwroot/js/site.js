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

            // ---- Generic edit modal population ----
            // Usage: trigger button has data-edit='{"Id":5,"Nom":"...","url":"/Controller/Edit/5"}'
            var editJson = trigger.getAttribute("data-edit");
            if (editJson) {
                var data;
                try {
                    data = JSON.parse(editJson);
                } catch (e) {
                    data = null;
                }

                if (data) {
                    var form = modal.querySelector("form");
                    if (form && data.url) {
                        form.setAttribute("action", data.url);
                    }

                    Object.keys(data).forEach(function (key) {
                        if (key === "url") return;
                        var field = modal.querySelector('[name="' + key + '"]');
                        if (field) {
                            field.value = data[key];
                        }
                    });
                }
            }

            // ---- Stash restore data for the "Undo" toast after a delete ----
            // Usage: delete-trigger button has data-restore='{"Nom":"...", ...}' (same shape as data-edit, no Id/url)
            var restoreJson = trigger.getAttribute("data-restore");
            if (restoreJson) {
                modal.setAttribute("data-pending-restore", restoreJson);
            } else {
                modal.removeAttribute("data-pending-restore");
            }
        });
    });

    // ---- Anti double-soumission : état de chargement sur le bouton submit ----
    document.addEventListener("submit", function (event) {
        var form = event.target;
        if (!(form instanceof HTMLFormElement) || form.hasAttribute("data-no-loading")) return;

        var submitBtn = form.querySelector('button[type="submit"]');
        if (!submitBtn || submitBtn.hasAttribute("data-loading")) return;

        // Attendre le prochain tick : laisse la validation (native ou jQuery unobtrusive)
        // annuler l'événement avant d'afficher l'état de chargement.
        setTimeout(function () {
            if (event.defaultPrevented) return;
            submitBtn.setAttribute("data-loading", "true");
            submitBtn.disabled = true;
            // Filet de sécurité : ne jamais rester bloqué si la navigation n'a pas lieu.
            setTimeout(function () {
                submitBtn.removeAttribute("data-loading");
                submitBtn.disabled = false;
            }, 8000);
        }, 0);
    });

    // ---- Capture des données de restauration juste avant l'envoi d'une suppression ----
    document.addEventListener("submit", function (event) {
        var form = event.target;
        if (!(form instanceof HTMLFormElement) || !form.hasAttribute("data-delete-form")) return;

        var modal = form.closest(".modal[id]");
        var restoreJson = modal ? modal.getAttribute("data-pending-restore") : null;
        if (!restoreJson) return;

        var action = form.getAttribute("action") || "";
        var controller = action.split("/").filter(Boolean)[0];
        if (!controller) return;

        try {
            var restore = JSON.parse(restoreJson);
            sessionStorage.setItem("gs-last-delete", JSON.stringify({ controller: controller, restore: restore, ts: Date.now() }));
        } catch (e) {
            /* ignore malformed payload */
        }
    });

    // ---- Toasts : auto-dismiss, fermeture manuelle, et action "Annuler" après suppression ----
    (function () {
        var container = document.getElementById("gsToastContainer");
        if (!container) return;

        function dismiss(toast) {
            toast.classList.add("gs-toast-hide");
            setTimeout(function () {
                toast.remove();
            }, 250);
        }

        var toasts = Array.prototype.slice.call(container.querySelectorAll(".gs-toast"));

        toasts.forEach(function (toast) {
            var closeBtn = toast.querySelector("[data-toast-close]");
            if (closeBtn) {
                closeBtn.addEventListener("click", function () {
                    dismiss(toast);
                });
            }
        });

        // Si une suppression vient d'aboutir et qu'une restauration est disponible,
        // ajoute un bouton "Annuler" sur le toast de succès correspondant.
        var pendingRaw = sessionStorage.getItem("gs-last-delete");
        sessionStorage.removeItem("gs-last-delete");

        var undoToast = null;
        if (pendingRaw) {
            try {
                var pending = JSON.parse(pendingRaw);
                if (Date.now() - pending.ts < 15000) {
                    undoToast = toasts.find(function (t) {
                        return t.getAttribute("data-toast-type") === "success" && /supprimé/i.test(t.textContent);
                    });

                    if (undoToast) {
                        var actions = undoToast.querySelector("[data-toast-actions]");
                        var tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
                        if (actions && tokenInput) {
                            var undoBtn = document.createElement("button");
                            undoBtn.type = "button";
                            undoBtn.className = "btn btn-sm btn-outline-secondary";
                            undoBtn.textContent = "Annuler";
                            undoBtn.addEventListener("click", function () {
                                var restoreForm = document.createElement("form");
                                restoreForm.method = "post";
                                restoreForm.action = "/" + pending.controller + "/Create";
                                restoreForm.style.display = "none";

                                var addField = function (name, value) {
                                    var input = document.createElement("input");
                                    input.type = "hidden";
                                    input.name = name;
                                    input.value = value;
                                    restoreForm.appendChild(input);
                                };

                                addField("__RequestVerificationToken", tokenInput.value);
                                Object.keys(pending.restore).forEach(function (key) {
                                    addField(key, pending.restore[key]);
                                });

                                document.body.appendChild(restoreForm);
                                restoreForm.submit();
                            });
                            actions.appendChild(undoBtn);
                        }
                    }
                }
            } catch (e) {
                /* ignore malformed payload */
            }
        }

        toasts.forEach(function (toast) {
            var duration = toast === undoToast ? 10000 : 5000;
            setTimeout(function () {
                dismiss(toast);
            }, duration);
        });
    })();

    // ---- Recherche rapide (quick-nav) dans la topbar ----
    (function () {
        var input = document.getElementById("gsQuickNavInput");
        var results = document.getElementById("gsQuickNavResults");
        if (!input || !results) return;

        var links = Array.prototype.slice.call(document.querySelectorAll("#gsSidebar .gs-sidebar-link"));
        var entries = links.map(function (link) {
            return { text: link.textContent.trim(), href: link.getAttribute("href") };
        }).filter(function (e) {
            return e.text && e.href;
        });

        function render(term) {
            results.innerHTML = "";
            var matches = term
                ? entries.filter(function (e) { return e.text.toLowerCase().indexOf(term) !== -1; })
                : entries;

            if (matches.length === 0) {
                var empty = document.createElement("div");
                empty.className = "gs-quicknav-empty";
                empty.textContent = "Aucun résultat.";
                results.appendChild(empty);
                return;
            }

            matches.slice(0, 8).forEach(function (e) {
                var a = document.createElement("a");
                a.href = e.href;
                a.textContent = e.text;
                results.appendChild(a);
            });
        }

        input.addEventListener("focus", function () {
            render(input.value.trim().toLowerCase());
            results.classList.add("show");
        });
        input.addEventListener("input", function () {
            render(input.value.trim().toLowerCase());
            results.classList.add("show");
        });
        document.addEventListener("click", function (event) {
            if (!results.contains(event.target) && event.target !== input) {
                results.classList.remove("show");
            }
        });
        input.addEventListener("keydown", function (event) {
            if (event.key === "Escape") {
                results.classList.remove("show");
                input.blur();
            }
        });
    })();

    // ---- Export CSV côté client ----
    // Usage: <button data-csv-export data-csv-target="#myTable" data-csv-filename="etudiants.csv">
    document.querySelectorAll("[data-csv-export]").forEach(function (btn) {
        btn.addEventListener("click", function () {
            var table = document.querySelector(btn.getAttribute("data-csv-target"));
            if (!table) return;

            var filename = btn.getAttribute("data-csv-filename") || "export.csv";
            var rows = [];

            var headerCells = table.querySelectorAll("thead th");
            rows.push(Array.prototype.slice.call(headerCells).map(function (th) {
                return th.textContent.trim();
            }).filter(function (_, i, arr) { return i < arr.length - 1; }));

            table.querySelectorAll("tbody tr[data-row]").forEach(function (tr) {
                if (tr.style.display === "none") return;
                var cells = Array.prototype.slice.call(tr.querySelectorAll("td"));
                cells.pop(); // dernière colonne = actions, non exportée
                rows.push(cells.map(function (td) { return td.textContent.trim(); }));
            });

            var csv = rows.map(function (row) {
                return row.map(function (cell) {
                    var escaped = String(cell).replace(/"/g, '""');
                    return /[",\n]/.test(escaped) ? '"' + escaped + '"' : escaped;
                }).join(",");
            }).join("\r\n");

            var blob = new Blob(["﻿" + csv], { type: "text/csv;charset=utf-8;" });
            var url = URL.createObjectURL(blob);
            var link = document.createElement("a");
            link.href = url;
            link.download = filename;
            document.body.appendChild(link);
            link.click();
            link.remove();
            URL.revokeObjectURL(url);
        });
    });
})();
