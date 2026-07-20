document.addEventListener("click", function (event) {
  const reasonRow = event.target.closest("table.table-ssi-reason-pick tbody tr");
  if (reasonRow && !event.target.closest('input[type="radio"]')) {
    const radio = reasonRow.querySelector('input[type="radio"]');
    if (radio && !radio.disabled) {
      radio.checked = true;
      radio.dispatchEvent(new Event("change", { bubbles: true }));
    }
  }

  const row = event.target.closest("table.table-ssi-select tbody tr");
  if (!row) {
    return;
  }

  if (event.target.closest('input[type="checkbox"]')) {
    return;
  }

  const checkbox = row.querySelector('input[type="checkbox"]');
  if (!checkbox || checkbox.disabled) {
    return;
  }

  checkbox.checked = !checkbox.checked;
  checkbox.dispatchEvent(new Event("change", { bubbles: true }));
});

document.addEventListener("change", function (event) {
  const checkbox = event.target;
  if (!(checkbox instanceof HTMLInputElement)) {
    return;
  }

  if (checkbox.type === "radio" && checkbox.name === "DisabledReasonId") {
    const nameEl = document.getElementById("account-status-name");
    const descriptionEl = document.getElementById("account-status-description");
    const iconEl = document.getElementById("account-status-icon");
    if (nameEl) {
      nameEl.textContent = checkbox.getAttribute("data-reason-name") ?? "";
    }
    if (descriptionEl) {
      const description = checkbox.getAttribute("data-reason-description") ?? "";
      descriptionEl.textContent = description.length === 0 ? "" : " — " + description;
    }
    if (iconEl) {
      const isEnabled = checkbox.getAttribute("data-reason-enabled") === "true";
      iconEl.classList.toggle("ssi-status-ok", isEnabled);
      iconEl.classList.toggle("ssi-status-bad", !isEnabled);
      iconEl.textContent = isEnabled ? "\u2713" : "\u2715";
    }
    return;
  }

  if (checkbox.type !== "checkbox") {
    return;
  }

  if (checkbox.id === "LoanAccess") {
    syncLoanLevelAccessPanel();
    return;
  }

  const table = checkbox.closest("table.table-ssi-select");
  if (!table) {
    return;
  }

  refreshSelectSummary(table);
});

function syncLoanLevelAccessPanel() {
  const checkbox = document.getElementById("LoanAccess");
  const panel = document.getElementById("loan-level-access-panel");
  if (!checkbox || !panel) {
    return;
  }

  panel.classList.toggle("d-none", !checkbox.checked);
}

function refreshSelectSummary(table) {
  const summary = findSelectSummary(table);
  if (!summary) {
    return;
  }

  const selectedCount = table.querySelectorAll('tbody input[type="checkbox"]:checked').length;
  if (selectedCount <= 0) {
    summary.hidden = true;
    summary.textContent = "";
    return;
  }

  const oneTemplate = summary.getAttribute("data-ssi-select-one") ?? "";
  const manyTemplate = summary.getAttribute("data-ssi-select-many") ?? "";
  const template = selectedCount === 1 ? oneTemplate : manyTemplate;
  summary.textContent = template.replaceAll("{count}", String(selectedCount));
  summary.hidden = false;
}

function findSelectSummary(table) {
  const previous = table.previousElementSibling;
  if (previous && previous.classList.contains("ssi-select-summary")) {
    return previous;
  }

  return null;
}

document.querySelectorAll("table.table-ssi-select").forEach(refreshSelectSummary);
syncLoanLevelAccessPanel();

